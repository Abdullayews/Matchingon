Matchingon

Matchingon is a memory and matching game developed using C# Windows Forms. The objective of the game is to find all 32 pairs of images on the board using as few attempts and as little time as possible.
Key Features

    Dynamic Game Board: An 8x8 game grid equipped with A-H and 1-8 border labels for easy coordinate identification.

    Statistics Panel: Real-time tracking of the number of attempts and elapsed time (minutes:seconds).

    Pair Management: Displays the number of matched pairs out of the 32 total pairs.

    Efficiency Calculation: Calculates an efficiency percentage at the end of the game based on the total attempts.

    Streamlined Flow: Once the game is won, an information window shows the results and closes the application upon confirmation.

Technologies & Requirements

    Programming Language: C#

    Framework: Windows Forms .NET

    IDE: Microsoft Visual Studio

File Structure

    Form1.cs: Contains the main game logic, card shuffling, click events, and match-checking mechanisms.

    Form1.Designer.cs: Auto-generated design code for the user interface components.

    GameButton.cs: A custom button class used to store the state of the cards (Row, Col, CardIndex, IsFlipped, IsMatched).

Setup and Execution

    Open the project in Microsoft Visual Studio.

    Ensure you have the images card1 through card32 added to your Properties.Resources.

    Rebuild the solution and run the project using Start.
