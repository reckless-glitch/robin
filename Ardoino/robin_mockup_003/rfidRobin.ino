/* Include the RFID library */
#include <RFID.h>

/* Define the DIO used for the SDA (SS) and RST (reset) pins. */
#define SDA_DIO 9
#define RESET_DIO 8
/* Create an instance of the RFID library */
RFID RC522(SDA_DIO, RESET_DIO);

#define MAX_TOKENS 7
int tokenIDs[MAX_TOKENS] = {144, 74, 162, 33, 241, 11, 249};
String tokens[MAX_TOKENS] = {"X", "A", "B", "C", "D", "E", "F"};
char keys[MAX_TOKENS] = {'X', 'A', 'B', 'C', 'D', 'E', 'F'};


int lastID = -1;
int loops = 0;
int waitLoops = 3;

void setupRfid()
{
  /* Initialise the RFID reader */
  RC522.init();
}

int printSN()
{
  for (int i = 0; i < 64; i++)
  {
    Serial.print(RC522.serNum[i], DEC);
    Serial.print(".");
    //Serial.print(RC522.serNum[i],HEX); //to print card detail in Hexa Decimal format
  }
}

String idToToken (int ID)
{
  for (int i = 0; i < MAX_TOKENS; i++)
  {
    if ( ID == tokenIDs[i])
    {
      return tokens[i];
    }
  }
  return "?";
}

bool tokenValid(String token)
{
  if (token == "0" || token == "?")
  {
    return false;
  }
  return true;
}

void checkRFID()
{
  /* Has a card been detected? */
  if (RC522.isCard())
  {
    loops = 0;
    /* If so then get its serial number */
    RC522.readCardSerial();
    if (lastID == RC522.serNum[0])
    {
      return;
    }
    lastID = RC522.serNum[0];
    setTKstate(idToToken(lastID));
  }
  else
  {
    if (loops < waitLoops)
    {
      loops++;
    }
    else
    {
      setTKstate("0");
      lastID = -1;
    }

  }
}
