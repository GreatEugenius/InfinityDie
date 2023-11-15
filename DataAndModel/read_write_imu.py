import serial
import csv
import time

# Initialize serial connection (this is an example, adjust for your IMU and connection)
Data = serial.Serial()
Data.baudrate = 19200
Data.port = '/dev/tty.usbmodem1101'
Data.timeout = 1
Data.open()

def dataProcess(data):
    data = str(data)
    dataSet = []
    datapoint = ''
    for i in data:
        if i.isdigit() or i == '-':
            datapoint += i
        elif i == "," and canFloat(datapoint):
            dataSet.append(float(datapoint))
            datapoint = ''
    return dataSet

def canFloat(data):
    try:
        float(data)
        return True
    except:
        return False
# Function to read data from IMU
def read_imu_data():
    # if ser.in_waiting:
    line = Data.readline()
    # Parse the line into respective data fields
    # This will depend on the format of the data coming from the IMU
    data = dataProcess(line)
    return data
    # return None


for i in range(10):
    # Collect data
    data_list = []
    start_time = time.time()
    print(f"Move{i}")
    while len(data_list) <= 90:
        elapsed_time = time.time() - start_time
        # Ensure that we are collecting data over exactly 3 seconds
        data = read_imu_data()
        # print(data)
        if data:
            data_list.append(data)

    start_point = 151
    # Write to CSV file
    with open(f'data/imu_data_{start_point+i}.csv', 'w', newline='') as file:
        writer = csv.writer(file)
        writer.writerow(['ax', 'ay', 'az', 'gx', 'gy', 'gz', 'row', 'pitch'])  # Replace with your actual labels
        writer.writerows(data_list)

    with open(f'label/imu_label_{start_point+i}.csv', 'w', newline='') as file:
        writer = csv.writer(file)
        writer.writerow(['label'])  # Replace with your actual labels
        writer.writerow(['3'])