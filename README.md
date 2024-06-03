# Cuba at war: 1958 (Gwent-Pro)
Card game for the first-year Programming project in Computer Science 2024. The project rules and requirements are outlined in the gwent-pro.pdf file. The chosen theme was the Cuban Revolutionary War of 1956-1959, which led to the downfall of the Batista dictatorship. The game’s graphical interface was created in Unity. Upon completion of the deadline, the game will also be uploaded as a .exe and .apk file. I hope you find my work enjoyable!

## First steps: 
1. https://github.com/sebagonz106/1958-Gwent-pro-
2. https://github.com/sebagonz106/Gwent-pro
*I am sorry about the mess, i'm learning in the process :')

## Overall description of the project:
1. As soon as the game starts running, you will find yourself in a menu with a picture of Sierra Maestra as background. You can choose to turn down the music volume or lower the quality.
2. When you press the local multiplayer button, you will be able to pick the leader card you want to play with. Both players must choose one in order to start the game.
3. When the game starts, you will be in between rounds panel, seeing Batista and Fidel in front of each other, at both sides of a gwent board.
4. The first player to play will be Batista, because his army was the one who started Summer offensive in 1958.
5. Both players will have the option to randomly change two cards of their hands upon starting the game. They wont be able to use this perk after first turn has ended or after they make their turn valid.
6. In every turn you will be able to do 1 out of 3 things: play a card, use the leader skill or pass. Once you pass, you wont be able to play until next round.
7. The round will go on until both players have passed. Scores will be compared and a round winner will be announced, unless there is a tie.
8. The game will go on unti a player has won 2 rounds, or tied 2 rounds and won 1.
9. Upon victory, a video will be played; and the choice to turn back to main menu will be shown.

## Resources used:
1. **Visual Studio Professional 2019**
2. **Unity 2022.3.15f1**
3. **Gimp 2**
4. **Blender**
5. **Copilot**

## Structure (by its importance):
1. **Board:** Class in charge of controlling the game logic, which stores the two players and the weather line, because the effects of the cards played there affect both sides.
2. **Master Controller:** Storage for all commonly used methods and game elements needed in the develpment process. It's also in charge of controlling rounds and turns endings and begginings.
3. **Player:** Class that keeps a leader card, a hand and a battlefield.
4. **Battlefield:** Class where the basic process of the game is ran. It stores the lists of cards displayed in the board and the graveyard.
5. **Card:** little to say about this, you can check the definition to see more (There are plenty of classes wich inheritate from this class, and i would say are the base cells of the game).
6. **Utils:** Commonly used methods and declarations. In this .cs document I declared the delegate Effects and all the Enums used throughout the coding.
