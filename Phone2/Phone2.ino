
#include <LiquidCrystal.h>

const int rs = 22, en = 24, d4 = 29, d5 = 27, d6 = 25, d7 = 23;
LiquidCrystal lcd(rs, en, d4, d5, d6, d7);

int phoneHolder = 11;
int button = 12;
int lockRelay = A5;
int ringerRelay = A4;

int digit_1 = 10;
int digit_2 = 9;
int digit_3 = 8;
int digit_4 = 7;
int digit_5 = 6;
int digit_6 = 5;
int digit_7 = 4;
int digit_8 = 3;
int digit_9 = 2;

int asterisk = -1;
int hashtag = -2;
int lastDigit = -3;

int destinationCode[9] = {0,3,9,1,2,3,4,5,6};

bool pushTheButton = false;
bool pickUpThePhone = false;
bool entercode = false;
bool openthephone = false;

int code[9];
int count = 0;

void pickUpThePhoneInstructions()
{
  Serial.print("EnterCode\n");
  delay(1000);
  Serial.flush();
}

void openPhoneInstructions()
{
  Serial.print("OpenThePhone\n");
  delay(1000);
  Serial.flush();
}

bool checkCode()
{
  if (code[0] == destinationCode[0] &&
      code[1] == destinationCode[1] &&
      code[2] == destinationCode[2] &&
      code[3] == destinationCode[3] &&
      code[4] == destinationCode[4] &&
      code[5] == destinationCode[5] &&
      code[6] == destinationCode[6] &&
      code[7] == destinationCode[7] &&
      code[8] == destinationCode[8])
      {
        return true;
      }
  return false;
}

void reset()
{
  pushTheButton = false;
  pickUpThePhone = false;
  entercode = false;
  openthephone = false;

  for(int i=0;i<9;i++)
  {
    code[i] = -1;
  }
  count = 0;

  lcd.setCursor(0, 1);
  lcd.print("Press The Button");

  digitalWrite(digit_1,     HIGH);
  digitalWrite(digit_2,     HIGH);
  digitalWrite(digit_3,     HIGH);
  digitalWrite(digit_8,     HIGH);
  digitalWrite(lockRelay,   HIGH);
  digitalWrite(ringerRelay, LOW);
}

void setup() {
  // put your setup code here, to run once:

  pinMode(digit_1,     OUTPUT);
  pinMode(digit_2,     OUTPUT);
  pinMode(digit_3,     OUTPUT);
  pinMode(digit_8,     OUTPUT);
  pinMode(lockRelay,   OUTPUT);
  pinMode(ringerRelay, OUTPUT);
  
  pinMode(phoneHolder, INPUT_PULLUP);
  pinMode(button,      INPUT_PULLUP);
  pinMode(digit_4,     INPUT_PULLUP);
  pinMode(digit_5,     INPUT_PULLUP);
  pinMode(digit_6,     INPUT_PULLUP);
  pinMode(digit_7,     INPUT_PULLUP);
  pinMode(digit_9,     INPUT_PULLUP);

  Serial.begin(9600);
  
  lcd.begin(16, 2);
  lcd.setCursor(0, 0);
  lcd.print(" THEGARAGE : AI ");

  reset();
}

void printCode()
{
  char number[16];
  for(int i = 0;i<16;i++)
  {
    if( i < 9 && code[i]!= -1)
        number[i] = code[i] + '0';
    else
      number[i] = ' ';
  }
  String str(number);
  lcd.setCursor(0, 1);
  lcd.print(str);
}

void loop() {
  // put your main code here, to run repeatedly:
  if (pushTheButton)
  {
    if (digitalRead(button) == LOW)
    {
      reset();
    }
    else if(pickUpThePhone)
    {
      if(!entercode)
      {
        pickUpThePhoneInstructions();
        entercode = true;
      }
       digitalWrite(digit_1, LOW);
       if (digitalRead(digit_4) == LOW && lastDigit != 3)
       {
        lastDigit = 3;
        code[count++] = 3;
       // Serial.print("Num = 3\n");
       }
       if (digitalRead(digit_5) == LOW && lastDigit != 6)
       {
        lastDigit = 6;
        code[count++] = 6;
       // Serial.print("Num = 6\n");
       }
       if (digitalRead(digit_6) == LOW && lastDigit != 9)
       {
        lastDigit = 9;
        code[count++] = 9;
       // Serial.print("Num = 9\n");
       }
       /*if (digitalRead(digit_7) == LOW && lastDigit != hashtag)
       {
        lastDigit = hashtag;
        code[count++] = hashtag;
        Serial.print("Num = #\n");
       }*/
       digitalWrite(digit_1, HIGH);
       digitalWrite(digit_2, LOW);
       if (digitalRead(digit_4) == LOW && lastDigit != 2)
       {
        lastDigit = 2;
        code[count++] = 2;
       // Serial.print("Num = 2\n");
       }
       if (digitalRead(digit_5) == LOW && lastDigit != 5)
       {
        lastDigit = 5;
        code[count++] = 5;
      //  Serial.print("Num = 5\n");
       }
       if (digitalRead(digit_6) == LOW && lastDigit != 8)
       {
        lastDigit = 8;
        code[count++] = 8;
       // Serial.print("Num = 8\n");
       }
       if (digitalRead(digit_7) == LOW  && lastDigit != 0)
       {
        lastDigit = 0;
        code[count++] = 0;
      //  Serial.print("Num = 0\n");
       }
       digitalWrite(digit_2, HIGH);
       digitalWrite(digit_3, LOW);
       if (digitalRead(digit_4) == LOW  && lastDigit != 1)
       {
        lastDigit = 1;
        code[count++] = 1;
      //  Serial.print("Num = 1\n");
       }
       if (digitalRead(digit_5) == LOW && lastDigit != 4)
       {
        lastDigit = 4;
        code[count++] = 4;
      //  Serial.print("Num = 4\n");
       }
       if (digitalRead(digit_6) == LOW && lastDigit != 7)
       {
        lastDigit = 7;
        code[count++] = 7;
      //  Serial.print("Num = 7\n");
       }
      /* if (digitalRead(digit_7) == LOW && lastDigit != asterisk)
       {
        lastDigit = asterisk;
        code[count++] = asterisk;
        Serial.print("Num = *\n");
       }*/
       digitalWrite(digit_3, HIGH);
       printCode();
       if (count == 9)
       {
         if (checkCode() == true)
         {
           digitalWrite(lockRelay,LOW);
           openPhoneInstructions();
           lcd.setCursor(0, 1);
           lcd.print("Open The Phone! ");
           delay(20000);
           reset();
         }
         else
         {
           count = 0;
           lcd.setCursor(0, 1);
           lcd.print("WRONG TRY AGAIN!");
           for(int i=0;i<9;i++)
           {
               code[i] = -1;
           }
           delay(2000);
           //Serial.print("WRONG TRY AGAIN!);
         }
       }
       delay(100);
    }
    else
    {
      if(digitalRead(phoneHolder) == LOW)
      {
        //Serial.print("Phone\n");
        pickUpThePhone = true;
        lcd.setCursor(0, 1);
        lcd.print("Insert your code");
        digitalWrite(ringerRelay, LOW);
      }
      else
      {
        digitalWrite(ringerRelay, HIGH);
      }
    }
  }
  else
  {
    if (digitalRead(button) == LOW)
    {
      pushTheButton = true;
      lcd.setCursor(0, 1);
      lcd.print("Answer the call ");
      //Serial.print("Button\n");
    }
    delay(10);
  }
}
