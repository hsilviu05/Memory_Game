# Memory Game

A modern implementation of the classic Memory card game using C# and WPF, featuring user profiles, statistics tracking, and customizable game settings.

## Features

### User Management
- Create and manage user profiles
- Associate profile pictures with users
- Delete user accounts and associated data
- Secure user data storage

### Game Modes
- Standard 4x4 board
- Custom board sizes (2x2 to 6x6)
- Multiple image categories
- Time-limited gameplay

### Game Features
- Card matching mechanics
- Timer with countdown
- Move counter
- Automatic game state saving
- Pause/Resume functionality

### Statistics
- Track games played and won
- Record best completion times
- Calculate average moves per game
- View player statistics

### Categories
- Jordan 1
- Jordan 4
- Jordan 11

## Requirements

- Windows 10 or later
- .NET 8.0
- Visual Studio 2022 (recommended)

## Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/Memory_Game.git
```

2. Open the solution in Visual Studio:
```
Memory_Game.sln
```

3. Build and run the application

## How to Play

1. **Start the Game**
   - Launch the application
   - Create a new user or select an existing one
   - Click "Play" to begin

2. **Game Rules**
   - Match pairs of cards with the same image
   - Each turn, flip two cards
   - If they match, they remain face up
   - If they don't match, they flip back
   - Complete all pairs before time runs out

3. **Controls**
   - Click cards to flip them
   - Use the Game menu for options:
     - New Game: Start a fresh game
     - Save Game: Save current progress
     - Load Game: Continue a saved game
     - Board Size: Change game board dimensions
     - Statistics: View player statistics
     - Exit: Return to login screen

4. **Winning**
   - Match all card pairs before time runs out
   - Statistics are automatically updated
   - Best times and moves are recorded

## Project Structure

```
Memory_Game/
├── Common/           # Common utilities and converters
├── Model/            # Data models (Game, Card, User, etc.)
├── Services/         # Business logic services
├── Utilities/        # Helper classes
├── View/             # XAML views
├── ViewModel/        # View models
└── Images/           # Game images and categories
```

## Technical Details

- Built with WPF (Windows Presentation Foundation)
- Implements MVVM (Model-View-ViewModel) pattern
- Uses data binding for UI updates
- JSON-based data storage
- Relative path handling for images

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Images used in the game are for educational purposes only
- Special thanks to the WPF and .NET communities

## Support

For support, please open an issue in the GitHub repository or contact the development team.