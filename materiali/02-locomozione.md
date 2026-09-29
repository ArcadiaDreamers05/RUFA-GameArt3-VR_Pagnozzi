# Lezione 2 — Locomozione e comfort con XR Interaction Toolkit

## Cosa c'è già nel rig
Il progetto istanzia all'avvio il prefab **XR Origin (XR Rig)** degli Starter Assets. Contiene già tutto: apri il prefab (`Assets/Samples/XR Interaction Toolkit/3.6.1/Starter Assets/Prefabs/XR Origin (XR Rig).prefab`, doppio click) e guarda sotto `Locomotion`:
- `Move` → **Dynamic Move Provider**: movimento continuo con lo stick sinistro. `Move Speed` 2,5 m/s, `Enable Strafe` acceso, `Enable Fly` spento, `Use Gravity` acceso, direzione relativa alla testa (`Head Relative`) o alla mano.
- `Turn` → **Snap Turn Provider** (`Turn Amount` 45°, `Debounce Time` 0,5 s, `Enable Turn Around` acceso: stick indietro = 180°) e **Continuous Turn Provider** (`Turn Speed` 60°/s), che è spento di default.
- `Teleportation` → **Teleportation Provider**: riceve le richieste delle aree di teleport.
- `Gravity`, `Jump` (tasto A), `Grab Move` (spento), `Climb`.
- Su `Left Controller` e `Right Controller` → **Controller Input Action Manager**: `Smooth Motion Enabled` (a sinistra acceso, a destra spento) e `Smooth Turn Enabled` (spento su entrambi). Sono questi due interruttori a decidere cosa fa ogni stick: stick con movimento continuo, oppure stick con teleport + snap turn.

## Comandi di default sul Quest
| Azione | Comando |
|---|---|
| Camminare | Stick sinistro |
| Snap turn 45° | Stick destro a sinistra/destra |
| Giro di 180° | Stick destro indietro |
| Teleport | Stick destro in avanti: compare l'arco, rilascia per saltare (il grip annulla) |
| Afferrare | Grip |
| Usare / premere UI | Trigger |

## Il teleport ha bisogno di un'area
Il rig porta l'interactor di teleport, ma il pavimento deve dirgli "qui si può". Il `Floor` della scena di partenza ce l'ha già: selezionalo e guarda il componente `Teleportation Area` nell'Inspector. Su ogni altro pavimento che costruisci lo aggiungi a mano: `Add Component → Teleportation Area`, `Interaction Layer Mask` = solo **Teleport** (layer 31, nominato da Setup 3). Il campo `Teleportation Provider` può restare vuoto: viene trovato all'avvio.

## Dove si cambiano i parametri
Il rig in scena esiste solo durante il Play, quindi le modifiche vanno fatte **sul prefab**, non sull'istanza `(Clone)` nella Hierarchy (quelle si perdono all'uscita dal Play). Modifica il prefab, salva (Ctrl+S in Prefab Mode) e riprova.

## Parametri di comfort: cosa scegliere e perché
Il malessere nasce quando gli occhi vedono un movimento che il corpo non sente. Regole che funzionano:
- **Teleport** come modo principale di spostarsi: niente accelerazione, niente nausea. Obbligatorio nell'escape room, dove ci si sposta poco.
- **Movimento continuo** solo se il genere lo richiede (walking simulator): `Move Speed` tra 1,5 e 2,5 m/s, mai di più, `Enable Fly` spento, direzione `Head Relative`.
- **Snap turn** 30° o 45°, mai la rotazione continua: `Smooth Turn Enabled` resta spento.
- **Frame rate stabile**: sotto i 90 Hz ogni movimento peggiora (da lezione 5 in poi).
- **Vignetta** durante movimento e rotazione: arriva alla lezione 12 con `RUFA ▸ Setup ▸ 6`.
- Nella versione PC la velocità è fissa (3 m/s, mouse per guardare) e non fa parte della consegna.

## Provare
- Sul visore: **RUFA ▸ Build ▸ Quest (APK)** e `adb install -r`.
- Senza rifare la build: **RUFA ▸ Play mode ▸ VR con Quest Link** (spunta) e **Play**, con l'app Meta Quest Link attiva sul PC e il runtime OpenXR impostato su Meta. Togli la spunta quando hai finito, altrimenti il Play normale prova a inizializzare OpenXR.

## Consegna
In `Consegne/L02/`: gli screenshot dell'Inspector di `Move`, `Turn` e dei due `Controller Input Action Manager` con i valori scelti, e una riga che spiega la scelta in base al tuo genere. Il sandbox deve essere esplorabile sul visore con teleport, snap turn e, se lo usi, movimento continuo.
