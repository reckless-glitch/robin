void setupUtilities()
{
  // initialize digital pin LED_BUILTIN as an output.
  pinMode(LED_BUILTIN, OUTPUT);
}

void blink(int high, int low)
{
  digitalWrite(LED_BUILTIN, HIGH);   // turn the LED on (HIGH is the voltage level)
  delay(high);                       // wait for a second
  digitalWrite(LED_BUILTIN, LOW);    // turn the LED off by making the voltage LOW
  delay(low);
}

void flash(int flashes)
{
  for (int i = 0; i < flashes-1; i++)
  {
    blink(50,200);
  }
  blink(50,0);
}
