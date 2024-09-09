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

## Virtual player:
When selecting the Single Player option in the main menu, the user will be given the possibility to choose their faction and leader card, as well as those of their opponent. When the game scene loads and the round start is confirmed, the camera will be positioned from the user’s perspective and will not move from there until the round ends. From here, the user can play their turn and receive information about the move made by their opponent. For this functionality, a recursive algorithm (method *MyPlay*) was developed, which evaluates the best move by consecutively making 3 valid turns and observing which combination leaves the player in the best position on the game board, seeking the greatest possible difference between their score and that of the other player. If it does not find a sequence of moves that favors it, it decides to pass. Additionally, a series of conditionals were used throughout the method to ensure that the move made is the most appropriate according to the current game situation and the player it represents.

## Interpreter for card creation:
 From the main menu, it is possible to access the card creation menu, where, after specifying the path where the necessary materials for this process will be found, the existing effects and cards are displayed, and the possibility to load new ones from a text file in the specified path is offered (although it is possible to load more than one at a time, it is recommended to load them one by one to properly address any errors or warnings that may appear) or program them within the integrated console. The specifics of the domain-specific language (*DSL*) used can be found in the file ‘*GWENT++.pdf*’, which contains the guidance for this functionality. For design, playability, and fidelity to the selected main theme, the possibility of declaring a new faction and creating a deck for it was restricted. In the visualization of the created card, the data entered by the user (name, description, faction, range, power, and type) will be used to generate an image similar to those used in other cards of the game. Similarly, the expanded information will be displayed when clicked on the field or double-clicked in hand. If the evaluation of the effect throws any unexpected or previously warned error, the application will inform about the failure of the effect and continue functioning.
 
 ### The interpreter is implemented in a C# class library, which consists of several parts:
1. **Lexer**: Receives a string of text and converts it into a list of tokens to be used in the next step. It performs lexical analysis of the input, checking, among other things, if the received characters are valid. Regular expressions were used to a small extent to recognize identifiers, numbers, and symbols.
2. **Parser**: Receives a list of tokens and constructs the abstract syntax tree (*AST*) that will represent the program. It has a recursive descent structure and is responsible for the syntactic analysis of the input, checking the correct structuring of the code. It contains a sequence of methods that parse all the structures allowed by the *DSL*, and its flow is internally directed within them, although the program starts from a main method that generates only cards and effects.
3. **Nodes**: Developed based on the IStatement interface, although two types can be distinguished: statements and expressions; the distinction lies in that when evaluating the latter, we expect a return value. Initially, the use of the ‘*Visitor*’ design pattern was considered, but it was decided to add the pertinent methods for semantic checking and evaluation within each node; therefore, they cannot be described merely as information storage but as an autonomous organism capable of self-regulation within the program.
4. **Semantic Check and Evaluation**: Performed through recursive calls from the root node, called Input. Each node of the constructed tree calls the consequent method of its children, and so on. As main aspects of the *DSL*, it is worth highlighting that in the effect node, semantic checking is performed before in the card node, as this makes it visible to the evaluator of the latter node, and the evaluation is performed only when the card it is associated with is played; while the evaluation of the card node adds the obtained card to a static list of cards belonging to the class, which is then accessed by the main class *Interpreter*. 

### Link: https://github.com/sebagonz106/Gwent-Interpreter

## Historical Context: 
With Fulgencio Batista’s coup d’état in March 1952, tensions escalated among Cubans who believed the country needed political and socioeconomic transformations to reduce widespread poverty, illiteracy, political-administrative corruption, foreign capital penetration, and unemployment; and to stop seeing Cuba as a “neocolony” and the “brothel” of the United States. With this goal, in 1953 the “Centennial Generation” decided to attack the Moncada barracks in Santiago de Cuba, then retreat to the Sierra Maestra and continue hostilities from there. Despite the failure of the action, the group took refuge in Mexico to plan an armed expedition that would finally bring the fight to national territory. Under the direction of Fidel Castro and the newly created *Movimiento 26 de julio*, this idea materialized at the end of 1956 with the Granma yacht expedition. 
Once in the Sierra Maestra, the small and dispersed group that survived the landing began to reorganize and plan future guerrilla actions, which began to bear fruit throughout 1957. Meanwhile, in the cities, the clandestine struggle was developing and intensifying, with an increase in attacks and sabotage carried out by revolutionaries on places of interest to the tyranny. By 1958, the revolutionary struggle was constantly growing, leading to the outbreak of a General Strike in April, which quickly failed due to lack of planning and brutal repression by the tyranny. Batista’s army responded swiftly to this action with the FF Plan (Final Phase or End of Fidel), which involved a massive deployment of 10,000 well-armed soldiers in Oriente.