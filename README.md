# Kinksweeper
Minesweeper, but in kinky way 

This is a game of Minsweeper with some additional rules.

## Rules of the game
For basic rules - see traditional 'minesweeper'.  
Unlike traditional Minesweeper, finding a mine here does not mean the end of the game. 
Instead of this, you receive a punishment - the game randomly selects a tag from a list of kinky tags and shows your random X pictures from rule34.xxx website using this tag.
You are required to choose one of the pictures - it could be either what you like more, what picture you have to enjoy,
what your partner has to do with you, what you have to send your friend, or any other punishment you can imagine ^^.


## How to build it
Use net6.0 or later:
`dotnet build -c Release`


## How to publish it
Use net6.0 or later:
`dotnet publish .\Kinksweeper.sln -c Release -o publish -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained true -p:IncludeNativeLibrariesForSelfExtract=true -p:MyRuntimeIdentifier=win-x64`
