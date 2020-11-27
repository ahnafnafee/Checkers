![Title](/Miscellaneous/title.png?raw=true "Title")


[![Codacy Badge](https://app.codacy.com/project/badge/Grade/bcbf08c9d6af40b195ffc3e36938b66d)](https://www.codacy.com/gh/ahnafnafee/Checkers/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=ahnafnafee/Checkers&amp;utm_campaign=Badge_Grade)

## Introduction

### Purpose

The purpose of this document is to describe the implementation of the Checkers Party game described in the Checkers Party Requirements Document. This game is designed to play the game of checkers among remote players.



### Scope

This document describes the implementation details of the Checkers Party game. This game will consist of one singular, major function. It will allow players from anywhere to join a match where they can battle each other out on the game of checkers that we implemented. This document will not specify how the actual game of checkers is played out.



### Definitions, Acronyms, Abbreviations

**Pieces** - The tokens used to move across the checker’s board diagonally to capture opponent pieces.

**Board** - The main layout upon which the pieces move. It is an 8x8 chequered board with 12 pieces per side.

**Objective** - The tasks assigned for each player to move across the board.

**Server** - The main hub that connects the clients and communicates moves to and from the client.

**Client** - The workstation where the game is being played.



## Design Overview

### Description of Purpose

Checkers Party is a two-player game designed to be played remotely. When each player joins the game, they will be placed in a lobby until both players are ready to play. Once they are prepared to play, they will be taken to the main checkers game, with the board facing their respective side. The objective of each player will be to take out all their opponent’s pieces. 

The game will provide a medium for players to interactively play with each other in real-time from any remote location in the world. 



### Technologies Used

Checkers Party will be developed using the Unity game engine. Since our game will be developed using this engine, the entirety of the project depends on it. Unity provides an easy to use interface for beginners and helps us implement the multiplayer functionality far more efficiently than other game engines.

We will use the Photon PUN package from the Unity Asset Store to establish networking protocols within the game for the multiplayer functionality of the game. 

The target platform will be Microsoft Windows, and the development environments are the Unity Engine and Visual Studio 2019. 



### System Architecture

Figure 1 depicts the high-level system architecture. The system will be constructed from multiple distinct components:

- **Lobby Interface** - This interface will allow the two players in the game to join a lobby user interface before they can join the game. 
- **Game Interface** - The main environment/user interface where the game will be played out between the players in real-time.
- **Photon Package** - The interface for connecting to the network so that players can send player moves can be synced with each other.

![Figure 1](/Miscellaneous/Picture1.png?raw=true "Title")

*Figure 1: High-level System Architecture*



### System Overview

Figure 2 is the typical sequence of events that occur during the course of a session of gameplay.

![Figure 2](/Miscellaneous/Picture2.png?raw=true "Title")

*Figure 2: Gameplay Sequence Diagram*
