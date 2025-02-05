#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include <MFRC522.h> // Thư viện cho RFID
#include <Servo.h>


LiquidCrystal_I2C lcd(0x27, 16, 2);

#define RST_PIN 9
#define SS_PIN 10
MFRC522 rfid(SS_PIN, RST_PIN);

// Servo
Servo welcomeServo;
Servo goodbyeServo;

//int availableSlots = 0;  // Số slot trống

void setup() {
  Serial.begin(9600);        // Giao tiếp Serial
  lcd.begin(16, 2);          // Khởi tạo LCD
  lcd.backlight();           // Bật đèn nền LCD

  // RFID
  SPI.begin();
  rfid.PCD_Init();

  // Servo
  welcomeServo.attach(8);    // Servo cho "WELCOME"
  goodbyeServo.attach(7);    // Servo cho "GOODBYE"
  InitialServo();

  //LCD
  displayMessage("   Welcome to", "  Dung Parking");

}

void loop() {

  processSerialCommands();

  if (rfid.PICC_IsNewCardPresent() && rfid.PICC_ReadCardSerial()) {
    handleRFIDCard();
    rfid.PICC_HaltA();
  }

  

}

void processSerialCommands() {
  if (Serial.available() > 0) {
    String command = Serial.readStringUntil('\n'); // Đọc lệnh từ Serial
    command.trim(); // Xóa khoảng trắng

    if (command == "WELCOME") 
      { 
        openGate(welcomeServo, "    WELCOME!");
      }
    
    else if (command == "GOODBYE")
      {
        openGate(goodbyeServo, "    GOODBYE!");
      }
    
    else if (command == "DENY")
      {
        displayMessage("  Invalid card", "   Close gate");

        displayMessage("   Welcome to", "  Dung Parking");
      }
  }
}




void handleRFIDCard() {
  // In UID của thẻ ra Serial Monitor

  for (byte i = 0; i < rfid.uid.size; i++) {
    Serial.print(rfid.uid.uidByte[i] < 0x10 ? "0" : "");
    Serial.print(rfid.uid.uidByte[i], HEX); // In từng byte của UID dưới dạng HEX
  }
  Serial.println();
}

void openGate(Servo &servo,const String &message) {
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print( message);
  lcd.setCursor(0, 1);
  lcd.print("   Open gate!");

  servo.write(90);
  delay(5000);
  InitialServo();

  
  displayMessage("   Welcome to", "  Dung Parking");

}

void InitialServo()
{
  welcomeServo.write(0);     // Đóng cổng WELCOME
  goodbyeServo.write(180);     // Đóng cổng GOODBYE
}

void displayMessage(const String& line1,const String& line2){
   lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print(line1);
  lcd.setCursor(0, 1);
  lcd.print(line2);
  delay(3000); // Hiển thị trong 3 giây
}

