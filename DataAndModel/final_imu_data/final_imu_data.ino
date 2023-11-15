// I2C device class (I2Cdev) demonstration Arduino sketch for MPU6050 class
// 10/7/2011 by Jeff Rowberg <jeff@rowberg.net>
// Updates should (hopefully) always be available at https://github.com/jrowberg/i2cdevlib
//
// Changelog:
//     2011-10-07 - initial release

/* ============================================
I2Cdev device library code is placed under the MIT license
Copyright (c) 2011 Jeff Rowberg

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
===============================================
*/

// Arduino Wire library is required if I2Cdev I2CDEV_ARDUINO_WIRE implementation
// is used in I2Cdev.h
#include "Wire.h"

// I2Cdev and MPU6050 must be installed as libraries, or else the .cpp/.h files
// for both classes must be in the include path of your project
#include "I2Cdev.h"
#include "MPU6050.h"
// #include "MPU6050_6Axis_MotionApps20.h"

#define RAD_TO_DEG 57.295779513082320876798154814105   
#define DEG_TO_RAD 0.01745329251994329576923690768489 

// // class default I2C address is 0x68
// // specific I2C addresses may be passed as a parameter here
// // AD0 low = 0x68 (default for InvenSense evaluation board)
// // AD0 high = 0x69
MPU6050 accelgyro;

int16_t ax, ay, az;
int16_t rx, ry, rz;
int16_t temperature;

// uint8_t euler[3];
// uint8_t fifoBuffer[64];
// Quaternion q; 


double dt;
double currTime, lastTime;

double roll, pitch;
double yaw = 0;

//Low pass fillter
double alphaLPF = 0.2;
double alphaLPFA = 0.38;
double filteredAccelerationX = 0.0;
double filteredAccelerationY = 0.0;
double filteredAccelerationZ = 0.0;
double filteredGyroX = 0.0;
double filteredGyroY = 0.0;
double filteredGyroZ = 0.0;

double alpha = 0.98;

//Displacement and velocity
double vx = 0, vy = 0, vz = 0;
double dx = 0, dy = 0, dz = 0;
double vxLast = 0, vyLast = 0, vzLast = 0;
double dxLast = 0, dyLast = 0, dzLast = 0;
double dxTotal = 0, dyTotal = 0, dzTotal = 0;

//Zero motion
int zeroMotionCount = 0;
int zeroDisplacementCount = 0;

float acc_roll, acc_pitch;

#define LED_PIN 13

bool blinkState = false;

//Caculat acceleration and gyroscope drift
double offset[6] = {0};
void caculatDrift(int16_t total) {
  double *ret;
  int i;
  for(i = 0; i < total; i++) {
    accelgyro.getAcceleration(&ax, &ay, &az);
    accelgyro.getRotation(&rx, &ry, &rz);

    offset[0] += value2Acc((double)(ax)) + 9.8 * sin(pitch * DEG_TO_RAD); 
    offset[1] += value2Acc((double)(ay)) - 9.8 * sin(roll * DEG_TO_RAD);
    offset[2] += value2Acc((double)(az)) - 9.8 * cos(roll * DEG_TO_RAD) * cos(pitch * DEG_TO_RAD);

    offset[3] += (double)(rx);
    offset[4] += (double)(ry);
    offset[5] += (double)(rz);
  }
  for(i = 0; i < 6; i++) {
    offset[i] /= total;
  }
}

//Get real angular velocity
double value2Gyro(double v) {
  return v * 500 / 32768;
}

//Get real acceleration
double value2Acc(double v) {
  return v * 8 * 9.8 / 32768;
}

// Get roll by gravity
double getRoll() {
  accelgyro.getAcceleration(&ax, &ay, &az);
  double a = atan2((double)ay, (double)az);
  return a;
}

//Get pitch by gravity
double getPitch() {
  accelgyro.getAcceleration(&ax, &ay, &az);
  double a = atan2(-(double)ax, sqrt((double)ay * (double)ay + (double)az * (double)az));
  return a;
}

void setup() {
    // join I2C bus (I2Cdev library doesn't do this automatically)
    Wire.begin();

    // initialize serial communication
    // (38400 chosen because it works as well at 8MHz as it does at 16MHz, but
    // it's really up to you depending on your project)
    //57600
    Serial.begin(19200);

    // initialize device
    // Serial.println("Initializing I2C devices...");
    accelgyro.initialize();

    // verify connection
    // Serial.println("Testing device connections...");
    // Serial.println(accelgyro.testConnection() ? "MPU6050 connection successful" : "MPU6050 connection failed");

    // configure Arduino LED for
    pinMode(LED_PIN, OUTPUT);

    // Set up custom full acc and gyro for the final problem 4 and 5.
    accelgyro.setFullScaleAccelRange(MPU6050_ACCEL_FS_8);
    accelgyro.setFullScaleGyroRange(MPU6050_GYRO_FS_500); 

    roll = getRoll() * RAD_TO_DEG;
    pitch = getPitch() * RAD_TO_DEG;

    caculatDrift(500);

    lastTime = millis();
    
}

void loop() {
    // read raw accel/gyro measurements from device
    accelgyro.getAcceleration(&ax, &ay, &az);
    accelgyro.getRotation(&rx, &ry, &rz); 
    // accelgyro.dmpGetEuler(euler, int *q)

    currTime = millis();
    dt = (currTime - lastTime) / 1000;
    lastTime = currTime;

    double accX = value2Acc((double)(ax));
    double accY = value2Acc((double)(ay));
    double accZ = value2Acc((double)(az));

    // Serial.print(accX); Serial.print(",");
    // Serial.print(accY); Serial.print(",");
    // Serial.print(accZ); Serial.print(",");

    accX += 9.8 * sin(pitch * DEG_TO_RAD);
    accY -= 9.8 * sin(roll * DEG_TO_RAD) - offset[1];
    accZ -= 9.8 * cos(roll * DEG_TO_RAD) * cos(pitch * DEG_TO_RAD);

    // Serial.print(accX); Serial.print(",");
    // Serial.print(accY); Serial.print(",");
    // Serial.print(accZ); Serial.print(",");

    //Low pass filter for acceleration
    filteredAccelerationX = alphaLPF * accX + (1 - alphaLPF) * filteredAccelerationX; 
    filteredAccelerationY = alphaLPF * accY + (1 - alphaLPF) * filteredAccelerationY; 
    filteredAccelerationZ = alphaLPF * accZ + (1 - alphaLPF) * filteredAccelerationZ; 
    accX = filteredAccelerationX;
    accY = filteredAccelerationY;
    accZ = filteredAccelerationZ;

    //Detect zero motion
    if(sqrt(accX * accX + accY * accY + accZ * accZ) < 1.0){
      zeroMotionCount += 1;
      accX = 0;
      accY = 0;
      accZ = 0;
      if(zeroMotionCount >= 30){
        vx = 0;
        vy = 0;
        vz = 0;
        vxLast = 0;
        vyLast = 0;
        vzLast = 0;
        zeroMotionCount = 0;
        zeroDisplacementCount += 1;
      }
      if(zeroDisplacementCount >= 5){
        if(dx > 0 or dx < 0){
          dxLast = dx;
          dyLast = dy;
          dzLast = dz;
        }

        dx = 0;
        dy = 0;
        dz = 0;
        
        zeroDisplacementCount = 0;
      }
    }
    else {
      zeroDisplacementCount = 0;
      zeroMotionCount = 0;
    }

    //v = x cm/s
    vx += accX * 100 * dt;
    vy += accY * 100 * dt;
    vz += accZ * 100 * dt;

    vx = vx * alphaLPFA + vxLast * (1 - alphaLPFA);
    vy = vy * alphaLPFA + vyLast * (1 - alphaLPFA);
    vz = vz * alphaLPFA + vzLast * (1 - alphaLPFA);

    vxLast = vx;
    vyLast = vy;
    vzLast = vz;

    dx += 0.5 * (vx + vxLast) * dt;
    dy += 0.5 * (vy + vyLast) * dt;
    dz += 0.5 * (vz + vzLast) * dt;

    dxTotal += 0.5 * (vx + vxLast) * dt;
    dyTotal += 0.5 * (vy + vyLast) * dt;
    dzTotal += 0.5 * (vz + vzLast) * dt;

    // Serial.print(vx); Serial.print(",");
    // Serial.print(vy); Serial.print(",");
    // Serial.print(vz); Serial.print(",");
    // Serial.print(dx); Serial.print(",");
    // Serial.print(dy); Serial.print(",");
    // Serial.print(dxLast); Serial.print(",");

    double rx_g = value2Gyro((double)(rx) - offset[3]);
    double ry_g = value2Gyro((double)(ry) - offset[4]);
    double rz_g = value2Gyro((double)(rz) - offset[5]);

    filteredGyroX = alphaLPF * (double)rx_g + (1 - alphaLPF) * filteredGyroX;
    filteredGyroY = alphaLPF * (double)ry_g + (1 - alphaLPF) * filteredGyroY;
    filteredGyroZ = alphaLPF * (double)rz_g + (1 - alphaLPF) * filteredGyroZ;
    
    acc_roll = atan2((double)ay, (double)az) * RAD_TO_DEG;
    acc_pitch = atan2(-(double)ax, sqrt((double)ay * (double)ay + (double)az * (double)az)) * RAD_TO_DEG;

    //Low pass filter for roll and pitch
    roll = (roll + filteredGyroX * dt) * alpha + (1 - alpha) * acc_roll;
    pitch = (pitch + filteredGyroY * dt) * alpha + (1 - alpha) * acc_pitch;
    if(filteredGyroZ < 1 && filteredGyroZ > -1) filteredGyroZ = 0;
    yaw = yaw + filteredGyroZ * dt;
    // Serial.print(yaw); Serial.print(",");
    Serial.print(ax); Serial.print(",");
    Serial.print(ay); Serial.print(",");
    Serial.print(az); Serial.print(",");
    Serial.print(rx); Serial.print(",");
    Serial.print(ry); Serial.print(",");
    Serial.print(rz); Serial.print(",");
    Serial.print(roll); Serial.print(",");
    Serial.print(pitch); Serial.print(",");
    Serial.println("");
    
    // blink LED to indicate activity
    blinkState = !blinkState;
    digitalWrite(LED_PIN, blinkState);
}
