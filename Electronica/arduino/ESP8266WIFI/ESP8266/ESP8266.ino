/*  ----------------------------------------------------------------
    http://www.prometec.net/
    Prog_76_2
    
    Montando un servidor Web en WIFI mediante
    un WIFI ESP8266 y comandos AT  
    http://www.prometec.net/servidor-web-esp8266
--------------------------------------------------------------------  
*/
String ordenes[]=
    {  //"AT+RST",
       "AT+CWMODE=3",
       "AT+CWQAP",
       "AT+CWJAP=\"SSID\",\"contrase\"",
       "AT+CIFSR" ,
       "AT+CIPMUX=1",
       "AT+CIPSERVER=1,80",
       "END"				// Para reconocer el fin de los comandos AT
    };

void setup()
{   Serial3.begin(115200);
    Serial.begin(115200); 
    delay (1000);
    int index = 0;
    while(ordenes[index] != "END")
        {  Serial3.println(ordenes[index++]);
           while ( true)
             {   String s = GetLineWIFI();
                 if ( s!= "") Serial.println(s);
                 if ( s.startsWith("no change"))   
                         break;
                 if ( s.startsWith("OK"))   
                         break;
                 if ( s.startsWith("ready"))   
                         break;
                // if (millis()-T >10000) break;  
             }
          Serial.println("....................");
      } 
}
void loop() 
{  while (Serial3.available() >0 )
    {  char c = Serial3.read();
       if (c == 71) 
          {   Serial.println("Enviada Web Request");
              webserver();
              delay(500);
          }
   }
}

void http(String output)
{
    Serial3.print("AT+CIPSEND=0,");              // AT+CIPSEND=0, num
    Serial3.println(output.length());
    if (Serial3.find(">"))                       // Si recibimos la peticion del mensaje
       {    Serial.println(output);
            Serial3.println(output);            //Aqui va el string hacia el WIFI 
            delay(10);
            while ( Serial3.available() > 0 ) 
              { if (  Serial3.find("SEND OK") )    // Busca el OK en la respuesta del wifi
                      break; 
              }
       }
}
void webserver(void) 
    {    http("<!DOCTYPE HTML>");
         http("<html>");
         http("<head><title>LECTURAS ANALOGICAS.</title>");
         http("<meta http-equiv=\"refresh\" content=\"15\"></head>");
         http("<body><h1> Situacion Ambiente</h1>");
  
         for (int analogChannel = 0; analogChannel < 6; analogChannel++) 
         {
            int sensorReading = analogRead(analogChannel);
            http("analog input ");
            http( String(analogChannel));
            http(" is ");
            http(String(sensorReading));
            http("<br />");
          }
        http("<p><em> La página se actualiza cada 15 segundos.</em></p></body></html>");

       delay(1);
       Serial3.println("AT+CIPCLOSE=0");
       //delay(500);
    }
String GetLineWIFI()
   {   String S = "" ;
       if (Serial3.available())
          {    char c = Serial3.read(); ;
                while ( c != '\n' )            //Hasta que el caracter sea intro
                  {     S = S + c ;
                        delay(25) ;
                        c = Serial3.read();
                   }
                 return( S ) ;
          }
   }


