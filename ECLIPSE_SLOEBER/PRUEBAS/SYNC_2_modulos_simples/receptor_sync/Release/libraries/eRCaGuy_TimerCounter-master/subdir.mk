################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
CPP_SRCS += \
C:\Users\antonio\Documents\Arduino\libraries\eRCaGuy_TimerCounter-master\eRCaGuy_Timer2_Counter.cpp 

LINK_OBJ += \
.\libraries\eRCaGuy_TimerCounter-master\eRCaGuy_Timer2_Counter.cpp.o 

CPP_DEPS += \
.\libraries\eRCaGuy_TimerCounter-master\eRCaGuy_Timer2_Counter.cpp.d 


# Each subdirectory must supply rules for building sources it contributes
libraries\eRCaGuy_TimerCounter-master\eRCaGuy_Timer2_Counter.cpp.o: C:\Users\antonio\Documents\Arduino\libraries\eRCaGuy_TimerCounter-master\eRCaGuy_Timer2_Counter.cpp
	@echo 'Building file: $<'
	@echo 'Starting C++ compile'
	"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\tools\avr-gcc\7.3.0-atmel3.6.1-arduino7/bin/avr-g++" -c -g -Os -w -std=gnu++11 -fpermissive -fno-exceptions -ffunction-sections -fdata-sections -fno-threadsafe-statics -Wno-error=narrowing -MMD -flto -mmcu=atmega328p -DF_CPU=16000000L -DARDUINO=10812 -DARDUINO_AVR_UNO -DARDUINO_ARCH_AVR     -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\cores\arduino" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\variants\standard" -I"D:\PROGRAMAS\Sloeber\arduinoPlugin\packages\arduino\hardware\avr\1.8.5\libraries\VirtualWire" -I"C:\Users\antonio\Documents\Arduino\libraries\eRCaGuy_TimerCounter-master" -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -D__IN_ECLIPSE__=1 -x c++ "$<"   -o "$@"
	@echo 'Finished building: $<'
	@echo ' '


