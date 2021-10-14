# CrazyGuys Project
Le projet "CrazyGuys" est un Action-Plateformer à la troisième personne en multijoueur qui a un gameplay dynamique et qui est axé sur la physique du jeu, le but du jeu diffère selon les niveaux.

![Playa_Screen_2](https://user-images.githubusercontent.com/51683201/137364669-9606d39c-4c6b-40a2-a09b-08f433e81600.PNG)

La totalité des éléments du jeu ont été créés par moi même que ce soit les Model 3D, UI, les animations, Shaders, Textures, Scripts, Particles Effects, Son, Musique, etc…

"CrazyGuys" M'a surtout servi comme support au développement d’une API spécialisée sur la gestion de la partie multijoueur (le Net Code) codé en C# et que j’ai nommé "BambooNetCode", comme le montrent certains extraits vidéo du jeu, la partie multijoueur a été créée (from scratch) pour remédier à certains besoins et à la résolution de problèmes bien spécifiques.

Temp de développement : 4 mois

https://user-images.githubusercontent.com/51683201/137362757-4efe7506-d21f-4900-86dd-efefcb770b5f.mp4


## Pourquoi avoir créé une API ?

Ce qui m'a surtout motivé à créer "BambooNetCode" est l’envie d’apprendre davantage et une curiosité qui m'a poussé encore plus à approfondir mes connaissances dans le domaine, mais aussi car les solutions proposées par le marché existant tel que (Unet, Photon Network, Mirror, etc.…) ne répondez pas à mes besoins.

Aussi car j’avais besoin d’avoir un contrôle total de la partie “serveur” et des paquets échangés entre ce dernier et le client du joueur.

## L'avantage d’avoir créer "BambooNetCode" 

l’API est très modulaire, et réutilisable d’un projet à un autre, facilement exportable et sans trop se perdre dans l'implémentation de cette dernière.

Contrairement à la topologie en Peer To Peer(Player Server-Host) tel que Photon Network pour exemple, "BambooNetCode" se base sur une topologie en “Dedicated serveur” ce qui signifie que le serveur gère le gameplay, les groupes, les matchmaking, la logique du jeu, les interactions physiques, le déplacement des entités, les instances de jeu, la persistance des données, etc.…

Exemple sur ce diagram

![Diag_1](https://user-images.githubusercontent.com/51683201/137362940-b81fd6bd-b976-4177-b32e-8dc4462c0caf.png)

Cela évite les phénomènes de désynchronisation des joueurs et permet une réduction drastique des moyens de triches, et permet une scalabilité efficace du serveur tout en gardant une complexité du code qui est gérable.

## Les Modules

"BambooNetCode" se compose de plusieurs modules et chaque module a une tâche bien spécifique parmi eux :

### Le module de transport
C'est celui qui permet de maintenir la communication et l'échange des requêtes TCP/UDP entre le serveur et le client.

### Packets Reader/Writer
Comme son nom l'indique le module permet de lire ou d'écrire des paquets grâce à la sérialisation de données quand il s'agit d'écrire ou deserialisation quand c'est de la lectures et cela soit pour être utilisée par le module de transport et envoyer au client/serveur soit au modules des requêtes et dans ce cas la savoir de quelle requête il s'agit.

### Le module des requêtes
Ce module est un genre de dictionnaire qui contient toutes les requêtes et leur fonction d'exécution après réception de celle-ci.

### Le Groupes Manager
Permet la gestion des groupes entre joueurs comme :

* l’affectation du rôle de leader du groupe
* les invitations au groupe
* l’acceptation/refus à l'adhésion au groupe
* l’exclusion d’un membre du groupe

### Module du Matchmaking
Gère l’ensemble des parties de Jeu/Gameplay côté serveur et permet au groupes de joueur de rejoindre des salons de jeu sans être séparés, mais aussi le module gère les files d’attentes et l'équilibrage via le ping (la latence) des joueurs.

### Les instance de jeu
Une fois que le salon de jeu est complet, ce module intervient et génère une instance où sont simulées en temps-réel (Real-Time Simulation) les interactions physiques des joueurs avec l’environnement.

### Interpolation/Extrapolation et lag réconciliation
L’un des modules le plus complexe à coder car il a la responsabilité de lisser, de prédire et fluidifier les mouvements et rotations des différentes entités de l’instance.

https://user-images.githubusercontent.com/51683201/137363311-bfc656c2-b1e8-495c-9675-ca25c3c5f687.mp4

Voici certains extraits de scriptes : 

* [GroupeManager.cs](https://github.com/Extons/GameServer_Network_API/blob/018845ca7c383d7eefb6f4611e7b40741635eb0e/ScriptsExample/CharacterMovementScript.cs "GroupeManager.cs")
* [PacketsForger.cs](https://github.com/Extons/GameServer_Network_API/blob/018845ca7c383d7eefb6f4611e7b40741635eb0e/ScriptsExample/PacketsForger.cs "PacketsForger.cs")
* [CharacterController.cs](https://github.com/Extons/GameServer_Network_API/blob/018845ca7c383d7eefb6f4611e7b40741635eb0e/ScriptsExample/CharacterController.cs "CharacterController.cs")
