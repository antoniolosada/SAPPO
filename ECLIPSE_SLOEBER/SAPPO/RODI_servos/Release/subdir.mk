################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
CPP_SRCS += \
..\FiltroKalman.cpp \
..\PID_v2.cpp \
..\SoftwareServo.cpp \
..\UKF.cpp \
..\list.cpp \
..\node_cpp.cpp \
..\sloeber.ino.cpp 

LINK_OBJ += \
.\FiltroKalman.cpp.o \
.\PID_v2.cpp.o \
.\SoftwareServo.cpp.o \
.\UKF.cpp.o \
.\list.cpp.o \
.\node_cpp.cpp.o \
.\sloeber.ino.cpp.o 

CPP_DEPS += \
.\FiltroKalman.cpp.d \
.\PID_v2.cpp.d \
.\SoftwareServo.cpp.d \
.\UKF.cpp.d \
.\list.cpp.d \
.\node_cpp.cpp.d \
.\sloeber.ino.cpp.d 


# Each subdirectory must supply rules for building sources it contributes
FiltroKalman.cpp.o: ..\FiltroKalman.cpp
	@echo 'Building file: $<'
	@echo 'Starting C++ compile'
	"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\tools\avr-gcc\7.3.0-atmel3.6.1-arduino7/bin/avr-g++" -c -g -Os -w -std=gnu++11 -fpermissive -fno-exceptions -ffunction-sections -fdata-sections -fno-threadsafe-statics -Wno-error=narrowing -MMD -flto -mmcu=atmega2560 -DF_CPU=16000000L -DARDUINO=10812 -DARDUINO_AVR_MEGA2560 -DARDUINO_ARCH_AVR     -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\variants\mega" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\cores\arduino" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\VirtualWire" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2\utility" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\Kalman\1.1.0" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\SPI\src" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\BasicLinearAlgebra\3.5.0" -I"C:\Users\antonio\Documents\Arduino\libraries\PlotPlus" -I"C:\Users\antonio\Documents\Arduino\libraries\MeanFilterLib\src" -I"C:\Users\antonio\Documents\Arduino\libraries\FIR_filter\src" -I"C:\Users\antonio\Documents\Arduino\libraries\SingleEMAFilterLib\src" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\SoftwareSerial\src" -I"C:\Users\antonio\Documents\Arduino\libraries\ServoTimer2" -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -D__IN_ECLIPSE__=1 -x c++ "$<"   -o "$@"
	@echo 'Finished building: $<'
	@echo ' '

PID_v2.cpp.o: ..\PID_v2.cpp
	@echo 'Building file: $<'
	@echo 'Starting C++ compile'
	"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\tools\avr-gcc\7.3.0-atmel3.6.1-arduino7/bin/avr-g++" -c -g -Os -w -std=gnu++11 -fpermissive -fno-exceptions -ffunction-sections -fdata-sections -fno-threadsafe-statics -Wno-error=narrowing -MMD -flto -mmcu=atmega2560 -DF_CPU=16000000L -DARDUINO=10812 -DARDUINO_AVR_MEGA2560 -DARDUINO_ARCH_AVR     -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\variants\mega" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\cores\arduino" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\VirtualWire" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2\utility" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\Kalman\1.1.0" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\SPI\src" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\BasicLinearAlgebra\3.5.0" -I"C:\Users\antonio\Documents\Arduino\libraries\PlotPlus" -I"C:\Users\antonio\Documents\Arduino\libraries\MeanFilterLib\src" -I"C:\Users\antonio\Documents\Arduino\libraries\FIR_filter\src" -I"C:\Users\antonio\Documents\Arduino\libraries\SingleEMAFilterLib\src" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\SoftwareSerial\src" -I"C:\Users\antonio\Documents\Arduino\libraries\ServoTimer2" -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -D__IN_ECLIPSE__=1 -x c++ "$<"   -o "$@"
	@echo 'Finished building: $<'
	@echo ' '

SoftwareServo.cpp.o: ..\SoftwareServo.cpp
	@echo 'Building file: $<'
	@echo 'Starting C++ compile'
	"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\tools\avr-gcc\7.3.0-atmel3.6.1-arduino7/bin/avr-g++" -c -g -Os -w -std=gnu++11 -fpermissive -fno-exceptions -ffunction-sections -fdata-sections -fno-threadsafe-statics -Wno-error=narrowing -MMD -flto -mmcu=atmega2560 -DF_CPU=16000000L -DARDUINO=10812 -DARDUINO_AVR_MEGA2560 -DARDUINO_ARCH_AVR     -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\variants\mega" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\cores\arduino" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\VirtualWire" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2\utility" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\Kalman\1.1.0" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\SPI\src" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\BasicLinearAlgebra\3.5.0" -I"C:\Users\antonio\Documents\Arduino\libraries\PlotPlus" -I"C:\Users\antonio\Documents\Arduino\libraries\MeanFilterLib\src" -I"C:\Users\antonio\Documents\Arduino\libraries\FIR_filter\src" -I"C:\Users\antonio\Documents\Arduino\libraries\SingleEMAFilterLib\src" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\SoftwareSerial\src" -I"C:\Users\antonio\Documents\Arduino\libraries\ServoTimer2" -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -D__IN_ECLIPSE__=1 -x c++ "$<"   -o "$@"
	@echo 'Finished building: $<'
	@echo ' '

UKF.cpp.o: ..\UKF.cpp
	@echo 'Building file: $<'
	@echo 'Starting C++ compile'
	"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\tools\avr-gcc\7.3.0-atmel3.6.1-arduino7/bin/avr-g++" -c -g -Os -w -std=gnu++11 -fpermissive -fno-exceptions -ffunction-sections -fdata-sections -fno-threadsafe-statics -Wno-error=narrowing -MMD -flto -mmcu=atmega2560 -DF_CPU=16000000L -DARDUINO=10812 -DARDUINO_AVR_MEGA2560 -DARDUINO_ARCH_AVR     -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\variants\mega" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\cores\arduino" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\VirtualWire" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2\utility" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\Kalman\1.1.0" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\SPI\src" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\BasicLinearAlgebra\3.5.0" -I"C:\Users\antonio\Documents\Arduino\libraries\PlotPlus" -I"C:\Users\antonio\Documents\Arduino\libraries\MeanFilterLib\src" -I"C:\Users\antonio\Documents\Arduino\libraries\FIR_filter\src" -I"C:\Users\antonio\Documents\Arduino\libraries\SingleEMAFilterLib\src" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\SoftwareSerial\src" -I"C:\Users\antonio\Documents\Arduino\libraries\ServoTimer2" -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -D__IN_ECLIPSE__=1 -x c++ "$<"   -o "$@"
	@echo 'Finished building: $<'
	@echo ' '

list.cpp.o: ..\list.cpp
	@echo 'Building file: $<'
	@echo 'Starting C++ compile'
	"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\tools\avr-gcc\7.3.0-atmel3.6.1-arduino7/bin/avr-g++" -c -g -Os -w -std=gnu++11 -fpermissive -fno-exceptions -ffunction-sections -fdata-sections -fno-threadsafe-statics -Wno-error=narrowing -MMD -flto -mmcu=atmega2560 -DF_CPU=16000000L -DARDUINO=10812 -DARDUINO_AVR_MEGA2560 -DARDUINO_ARCH_AVR     -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\variants\mega" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\cores\arduino" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\VirtualWire" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2\utility" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\Kalman\1.1.0" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\SPI\src" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\BasicLinearAlgebra\3.5.0" -I"C:\Users\antonio\Documents\Arduino\libraries\PlotPlus" -I"C:\Users\antonio\Documents\Arduino\libraries\MeanFilterLib\src" -I"C:\Users\antonio\Documents\Arduino\libraries\FIR_filter\src" -I"C:\Users\antonio\Documents\Arduino\libraries\SingleEMAFilterLib\src" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\SoftwareSerial\src" -I"C:\Users\antonio\Documents\Arduino\libraries\ServoTimer2" -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -D__IN_ECLIPSE__=1 -x c++ "$<"   -o "$@"
	@echo 'Finished building: $<'
	@echo ' '

node_cpp.cpp.o: ..\node_cpp.cpp
	@echo 'Building file: $<'
	@echo 'Starting C++ compile'
	"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\tools\avr-gcc\7.3.0-atmel3.6.1-arduino7/bin/avr-g++" -c -g -Os -w -std=gnu++11 -fpermissive -fno-exceptions -ffunction-sections -fdata-sections -fno-threadsafe-statics -Wno-error=narrowing -MMD -flto -mmcu=atmega2560 -DF_CPU=16000000L -DARDUINO=10812 -DARDUINO_AVR_MEGA2560 -DARDUINO_ARCH_AVR     -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\variants\mega" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\cores\arduino" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\VirtualWire" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2\utility" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\Kalman\1.1.0" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\SPI\src" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\BasicLinearAlgebra\3.5.0" -I"C:\Users\antonio\Documents\Arduino\libraries\PlotPlus" -I"C:\Users\antonio\Documents\Arduino\libraries\MeanFilterLib\src" -I"C:\Users\antonio\Documents\Arduino\libraries\FIR_filter\src" -I"C:\Users\antonio\Documents\Arduino\libraries\SingleEMAFilterLib\src" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\SoftwareSerial\src" -I"C:\Users\antonio\Documents\Arduino\libraries\ServoTimer2" -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -D__IN_ECLIPSE__=1 -x c++ "$<"   -o "$@"
	@echo 'Finished building: $<'
	@echo ' '

sloeber.ino.cpp.o: ..\sloeber.ino.cpp
	@echo 'Building file: $<'
	@echo 'Starting C++ compile'
	"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\tools\avr-gcc\7.3.0-atmel3.6.1-arduino7/bin/avr-g++" -c -g -Os -w -std=gnu++11 -fpermissive -fno-exceptions -ffunction-sections -fdata-sections -fno-threadsafe-statics -Wno-error=narrowing -MMD -flto -mmcu=atmega2560 -DF_CPU=16000000L -DARDUINO=10812 -DARDUINO_AVR_MEGA2560 -DARDUINO_ARCH_AVR     -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\variants\mega" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\cores\arduino" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\VirtualWire" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2\utility" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\Kalman\1.1.0" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\SPI\src" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\BasicLinearAlgebra\3.5.0" -I"C:\Users\antonio\Documents\Arduino\libraries\PlotPlus" -I"C:\Users\antonio\Documents\Arduino\libraries\MeanFilterLib\src" -I"C:\Users\antonio\Documents\Arduino\libraries\FIR_filter\src" -I"C:\Users\antonio\Documents\Arduino\libraries\SingleEMAFilterLib\src" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\SoftwareSerial\src" -I"C:\Users\antonio\Documents\Arduino\libraries\ServoTimer2" -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -D__IN_ECLIPSE__=1 -x c++ "$<"   -o "$@"

	@echo 'Finished building: $<'
	@echo ' '


