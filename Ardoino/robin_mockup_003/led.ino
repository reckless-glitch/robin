#define LED_VOLTAGE 5


static const int scanner[] = {4,5,6,7};
static const uint8_t lights[] = { A0,A1,A2,A3,A4 };

int lightmode = 0;
int scannerLoopTime = 99;
int scannerFlashTime = 30;
int scannerNowTime = 0;
int ledLoopDelay = 50;
float scannerBright = 0.95f;
float lightBright = 1.000f;
  
void setupLED(int loopDelay)
{
  scannerBright =	constrain(scannerBright, 0, 1);
  lightBright = constrain(lightBright, 0, 1);
  for (int i = 0; i < 5; i++)	  pinMode(lights[i], OUTPUT);
  for (int i = 0; i < 3; i++)	  pinMode(scanner[i], OUTPUT);

  ledLoopDelay = loopDelay;
  ledStopScanning();
  //ledTest();
}

void ledTest()
{
	//lightmode = -2;
	//ledUpdate();
	//delay(200);
	lightmode = -1;
	ledUpdate();
}

void ledStartScanning()
{
	lightmode = 1;
}


void ledUpdate()
{
	if (lightmode == -2)
	{
		toggleScanner(0);
		switchLights(false);
	}
	if (lightmode == -1)
	{
		toggleScanner(-1);
		switchLights(true);
	}
	else if (lightmode == 0)
  {
		switchLights(true);
		toggleScanner(0);
  }
  else
  {
	  switchLights(false);
    if (scannerNowTime >= scannerLoopTime)
    {
		scannerNowTime = 0;
      lightmode++;
      if (lightmode > 4)
      {
        lightmode = 1;
      }
    }
    else
    {
		scannerNowTime += ledLoopDelay;
    }
    if (scannerNowTime < scannerFlashTime)
    {
      toggleScanner(lightmode);
    }
    else
    {
      toggleScanner(0);
    }
  }
}

void ledStopScanning()
{
	lightmode = 0;
}

void switchLights(bool on)
{
	for (int i = 0; i < 5; i++)
	{
		if(on) analogWrite(lights[i], 128);
		else analogWrite(lights[i], 0);
	}
}

void toggleScanner(int id)
{
	for (int i = 0; i < 4; i++)
	{
		if(id == -1 || i == id - 1)
		{
			analogWrite(scanner[i], scannerBright*255);
		}
		else
		{
			digitalWrite(scanner[i], LOW);
		}
	}
}
