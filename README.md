# Checkers
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/bcbf08c9d6af40b195ffc3e36938b66d)](https://www.codacy.com?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=ahnafnafee/Checkers&amp;utm_campaign=Badge_Grade)

### Introduction

Checkers Party is a two-player game designed to be played remotely. When each player joins the game, they will be placed in a lobby until both players are ready to play. Once they are prepared to play, they will be taken to the main checkers game, with the board facing their respective side. The objective of each player will be to take out all their opponent’s pieces. 

The game is intended to run on both of the player’s devices, which will act as the client. The central server may be hosted in an AWS EC2 instance, which will have an elastic IP. The primary client will be Windows/MAC PCs, and the players can control their actions using simple mouse clicks. 

The game is intended to be in 2D with a beautiful UI overlaid on it. The overall theme will be casual (think Solitaire), maybe also adding a timed mode for a competitive edge. 

### Activity Diagram

![](D:\GitHub\Checkers\Miscellaneous\activityDiagram.png)