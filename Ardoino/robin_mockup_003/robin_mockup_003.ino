

/*
  PINOUT:
  RC522 MODULE    Uno/Nano     MEGA
  SDA             D10          D9
  SCK             D13          D52
  MOSI            D11          D51
  MISO            D12          D50
  IRQ             N/A          N/A
  GND             GND          GND
  RST             D9           D8
  3.3V            3.3V         3.3V
*/
/* Include the standard Arduino SPI library */
#include <SPI.h>

#define LOOPDELAY  50
String btState = "0";
String tkState = "0";
String tkTriggered = "0";

void setBTstate (bool state)
{
  if (state)
  {
    btState = "1";
  }
  else
  {
    btState = "0";
  }
}

void setTKstate (String token)
{
  if (tkState == token)
  {
    return;
  }
  if (tkTriggered != token && tokenValid(token))
  {
    tkTriggered = token;
  }
  tkState = token;
}

void reset()
{
	tkTriggered = "0";
	ledTest();
	delay(200);
	ledStopScanning();
}


void setup()
{
  /* Enable the SPI interface */
  SPI.begin();
  setupUtilities();
  setupRfid();
  setupUnity();
  //setupBT();
  pinMode(LED_BUILTIN, OUTPUT);
  //setupLED(LOOPDELAY);
}

bool buildinLED = false;

void loop()
{
	buildinLED = !buildinLED;
	if(buildinLED) digitalWrite(LED_BUILTIN, HIGH); 
	else digitalWrite(LED_BUILTIN,LOW);
  checkRFID();
  updateUnity();
  ledUpdate();
  //loopBT();
  delay(LOOPDELAY);
}
