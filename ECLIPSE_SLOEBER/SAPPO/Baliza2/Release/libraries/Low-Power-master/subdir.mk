################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
CPP_SRCS += \
C:\Users\antonio\Documents\Arduino\libraries\Low-Power-master\LowPower.cpp 

LINK_OBJ += \
.\libraries\Low-Power-master\LowPower.cpp.o 

CPP_DEPS += \
.\libraries\Low-Power-master\LowPower.cpp.d 


# Each subdirectory must supply rules for building sources it contributes
libraries\Low-Power-master\LowPower.cpp.o: C:\Users\antonio\Documents\Arduino\libraries\Low-Power-master\LowPower.cpp
	@echo 'Building file: $<'
	@echo 'Starting C++ compile'
	"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\tools\avr-gcc\7.3.0-atmel3.6.1-arduino7/bin/avr-g++" -c -g -Os -w -std=gnu++11 -fpermissive -fno-exceptions -ffunction-sections -fdata-sections -fno-threadsafe-statics -Wno-error=narrowing -MMD -flto -mmcu=atmega328p -DF_CPU=16000000L -DARDUINO=10812 -DARDUINO_AVR_UNO -DARDUINO_ARCH_AVR     -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\cores\arduino" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\variants\standard" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\SPI\src" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\VirtualWire" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\libraries\RF24\1.4.2\utility" -I"C:\Users\antonio\Documents\Arduino\libraries\Low-Power-master" -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -D__IN_ECLIPSE__=1 -x c++ "$<"   -o "$@"
	@echo 'Finished building: $<'
	@echo ' '


