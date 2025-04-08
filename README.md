# 🧠 Memory Game

A modern implementation of the classic Memory card game using C# and WPF, featuring user profiles, statistics tracking, and customizable game settings.

---

## 📌 Features

### 👤 User Management
- Create and manage user profiles
- Associate profile pictures with users
- Delete user accounts and associated data
- Secure user data storage

### 🎮 Game Modes
- Standard 4x4 board
- Custom board sizes: 2x2 to 6x6
- Multiple image categories: Jordan 1, Jordan 4, Jordan 11
- Time-limited gameplay with countdown

### 🃏 Game Features
- Smooth card flipping and matching mechanics
- Timer with countdown and pause/resume
- Move counter and game completion tracker
- Auto-save and resume functionality

### 📊 Statistics
- Track total games played and won
- Best times recorded per board size
- Average moves per game
- Per-user statistics view

---

## ⚙️ Requirements

- Windows 10 or later
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 (recommended)

---

## ▶️ How to Play

1. **Start the Game**
   - Launch the application
   - Create a user or select an existing one
   - Click **Play** to begin

2. **Game Rules**
   - Flip two cards per turn
   - Match identical images to keep cards face-up
   - Mismatched cards flip back
   - Complete the board before time runs out

3. **Controls**
   - Click on cards to flip them
   - Use the **Game** menu for:
     - `New Game`: Start over
     - `Save Game`: Save current progress
     - `Load Game`: Resume a saved session
     - `Board Size`: Choose board dimensions
     - `Statistics`: View player progress
     - `Exit`: Return to login screen

4. **Winning**
   - Match all card pairs within the time limit
   - Statistics are saved automatically
   - Top times and fewest moves are tracked

---

## 🗂️ Project Structure

Memory_Game/ 
├── Common/ # Common utilities and converters 
├── Model/ # Data models (Game, Card, User, etc.) 
├── Services/ # Business logic services 
├── Utilities/ # Helper classes 
├── View/ # XAML UI screens and game views 
├── ViewModel/ # MVVM view models 
└── Images/ # Game assets and image categories

---

## 🔧 Technical Details

- Built with **WPF (Windows Presentation Foundation)**
- Clean architecture with **MVVM pattern**
- Live UI updates via **data binding**
- Game and user data stored using **JSON**
- Dynamic and relative path handling for assets

---

## 📸 Screenshots

## Start Game
![image](https://github.com/user-attachments/assets/6854a5b7-0de6-4e02-8b1a-414fc200cc52)

## Login Menue
![image](https://github.com/user-attachments/assets/2f948eee-6375-4c11-bec7-020fd76c331f)

## Game 
![image](https://github.com/user-attachments/assets/c01c1d09-9a5f-4d56-8daa-5eaf8f241456)

## 🎉 WIN 
![image](https://github.com/user-attachments/assets/76e70b8c-e3b8-4e22-a75b-75f5f7987ed0)

