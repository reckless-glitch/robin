#include <SerialCommand.h>

SerialCommand sCmd;


void setupUnity()
{
  Serial.begin(9600);
  while (!Serial);

  sCmd.addCommand("state", stateHandler);
  sCmd.addCommand("start", startHandler);
  sCmd.addCommand("stop", stopHandler);
  sCmd.addCommand("reset", resetHandler);
  //sCmd.setDefaultHandler(errorHandler);
  // initialize digital pin LED_BUILTIN as an output.
}

void stateHandler()
{
  Serial.println("state:"+btState + "/" + tkState + "/" + tkTriggered );
  Serial.println("EOM");
}

void startHandler()
{
	Serial.println("start");
	ledStartScanning();
}

void stopHandler()
{
	Serial.println("stop");
	ledStopScanning();
}

void resetHandler()
{
	Serial.println("reset robin");
	reset();

}

void updateUnity()
{
  if (Serial.available() > 0)
  {
    sCmd.readSerial();
  }
}
