################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
CPP_SRCS += \
C:\Users\antonio\Documents\Arduino\libraries\Servo\src\avr\Servo.cpp 

LINK_OBJ += \
.\libraries\Servo\src\avr\Servo.cpp.o 

CPP_DEPS += \
.\libraries\Servo\src\avr\Servo.cpp.d 


# Each subdirectory must supply rules for building sources it contributes
libraries\Servo\src\avr\Servo.cpp.o: C:\Users\antonio\Documents\Arduino\libraries\Servo\src\avr\Servo.cpp
	@echo 'Building file: $<'
	@echo 'Starting C++ compile'
	"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\tools\avr-gcc\7.3.0-atmel3.6.1-arduino7/bin/avr-g++" -c -g -Os -w -std=gnu++11 -fpermissive -fno-exceptions -ffunction-sections -fdata-sections -fno-threadsafe-statics -Wno-error=narrowing -MMD -flto -mmcu=atmega2560 -DF_CPU=16000000L -DARDUINO=10812 -DARDUINO_AVR_MEGA2560 -DARDUINO_ARCH_AVR     -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\variants\mega" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\cores\arduino" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\VirtualWire" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2\utility" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\Kalman\1.1.0" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\SPI\src" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\BasicLinearAlgebra\3.5.0" -I"C:\Users\antonio\Documents\Arduino\libraries\PlotPlus" -I"C:\Users\antonio\Documents\Arduino\libraries\MeanFilterLib\src" -I"C:\Users\antonio\Documents\Arduino\libraries\FIR_filter\src" -I"C:\Users\antonio\Documents\Arduino\libraries\SingleEMAFilterLib\src" -I"C:\Users\antonio\Documents\Arduino\libraries\Servo\src" -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -D__IN_ECLIPSE__=1 -x c++ "$<"   -o "$@"
	@echo 'Finished building: $<'
	@echo ' '


