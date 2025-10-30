# 🍳 Kitchen Clash Online – Multiplayer Cooking Simulation

## 📝 About
*A fully networked multiplayer cooking game built in Unity, featuring modular gameplay systems, scalable clean code, and full Netcode for GameObjects integration. Up to four players compete or collaborate in real time to prepare and deliver recipes, complete with synchronized interactions, custom inputs, and immersive audiovisual feedback.*

---

## 🎮 Overview
**Kitchen Clash Online** is a multiplayer cooking simulation game developed in Unity (C#) that combines advanced networking, clean architecture, and reactive gameplay design.  
Players join online lobbies, select their characters, and enter a fast-paced kitchen where they must cook, cut, fry, and deliver dishes to win.  
The project demonstrates professional-level usage of **Unity Netcode for GameObjects**, **Relay & Lobby services**, and a modular, object-oriented gameplay architecture.

---

## 🔑 Key Features

### 🎯 Gameplay Systems
- **Interactive cooking stations**: Cutting, frying, combining ingredients, and plating systems.  
- **Real-time synchronization** of every action across the network using RPCs and `NetworkVariable`.  
- **Dynamic recipe system** powered by ScriptableObjects (`CuttingObjectSO`, `FryingObjectSO`, etc.).  
- **Player interaction logic**: pickup, drop, and combination of ingredients.  
- **Progress tracking** and visual feedback for each cooking phase.  
- **Delivery validation and scoring** via `DeliveryManager`.

### 🌐 Multiplayer & Networking
- Full **online multiplayer (1–4 players)** with Unity Relay and Lobby Services.  
- **Lobby system** with player color selection, ready/unready state, and kick management.  
- **Authoritative server-client architecture** using `ServerRpc` and `ClientRpc`.  
- **Network object spawning and destruction** handled through `KitchenObject` and `KitchenGameManager`.  
- **Automatic player spawning** when scenes load via Netcode’s SceneManager events.

### ⚙️ Architecture & Code Quality
- **Clean, scalable C# code** following SOLID and event-driven design principles.  
- **BaseCounter system** with modular subclasses: Cutting, Stove, Plates, Clear.  
- **Interfaces** (`IKitchenObject`, `IHasProgress`) for decoupled gameplay systems.  
- **ScriptableObject-driven design**, allowing flexible recipes and behavior changes without editing code.  
- **Persistent Input System** with rebinding support for both keyboard and controller.  
- **Comprehensive UI layer**: lobby, character select, countdown, pause, HUD.

### 🔊 Audio & Feedback
- Centralized **SoundManager** reacting to all gameplay events (cutting, frying, delivery, etc.).  
- **3D spatial audio** for immersive feedback.  
- **Persistent volume settings** saved with PlayerPrefs.  
- Optional **visual shaders and particle effects** for cooking feedback.

---

## 🧩 Technical Stack
| Category | Technology |
|-----------|-------------|
| **Engine** | Unity 2022+ |
| **Language** | C# |
| **Networking** | Unity Netcode for GameObjects (NGO), Relay, Lobby Services |
| **Input** | Unity Input System (rebinding supported) |
| **Data** | ScriptableObjects |
| **UI** | Unity UI + TextMeshPro |
| **Architecture** | Event-driven, modular, network-synced |

---

## 🧠 Core Systems Breakdown
| System | Description |
|--------|--------------|
| **KitchenGameManager** | Handles game states (Waiting → Countdown → Playing → Game Over). |
| **DeliveryManager** | Validates recipes, tracks scores, synchronizes success/fail events. |
| **Player** | Manages movement, interactions, and object handling per client. |
| **BaseCounter & Subclasses** | Modular station system (Cutting, Stove, Plates, Clear). |
| **KitchenObject** | Core network object for all interactable items. |
| **Lobby & Multiplayer** | Lobby creation, joining, color selection, ready system. |
| **SoundManager** | Central audio hub reacting to gameplay events. |
| **UI Systems** | Lobby UI, character select, progress indicators, timer HUD. |

---

## 🧪 Notable Technical Highlights
- Advanced **RPC synchronization** for time-based mechanics (cooking, cutting, frying).  
- Smart **parent-child assignment** for all network objects (`KitchenObjectParent`).  
- **Client-side prediction** and authority-safe logic to prevent desync.  
- **Event-based architecture** to trigger sounds, VFX, and UI updates seamlessly.  
- Highly modular, **extensible system** for adding new gameplay mechanics.

---

## 🚀 Future Improvements
- Add **voice chat** for better team coordination.  
- Implement **AI-controlled bots** for solo or practice mode.  
- Expand cooking mechanics (boiling, baking, chopping upgrades).  
- Introduce **player customization** and cosmetic unlocks.  
- Integrate **matchmaking** and ranked competitive mode.

---

## 🕹️ How to Play
1. Launch the game and connect to the lobby system.  
2. Create or join a lobby (up to 4 players).  
3. Select your character and press *Ready*.  
4. Once all players are ready, the game begins!  
5. Pick up ingredients, cook, combine, and deliver dishes before the timer runs out.  
6. The player (or team) with the most successful recipes wins.

---

## Media
![Paraborrar2025-10-3000-50-31-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/3056a24f-70b7-48f3-ad52-ade51afeacbe)


---

## 💡 Lessons Learned
This project showcases deep knowledge of:
- Multiplayer architecture and synchronization using Unity Netcode.  
- Scalable, maintainable game system design with C#.  
- Efficient event-driven gameplay programming.  
- Integration of Unity’s modern systems (Input, Lobby, Relay, ScriptableObjects).  
- Building production-ready foundations for online cooperative experiences.

---

### 🧑‍💻 Author
Developed by **[Your Name]** – Game Developer & Programmer  
*(Portfolio link / GitHub profile)*  

---

