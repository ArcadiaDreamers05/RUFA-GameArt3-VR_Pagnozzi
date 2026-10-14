# Lezione 2 — Locomozione e comfort con XR Interaction Toolkit

## Cosa impari
Com'è fatto il rig VR di XR Interaction Toolkit 3 (XR Origin, tracking, input dei controller, locomozione) e tutti i modi di far muovere il giocatore: camminare, girare, teletrasportarsi, saltare, arrampicarsi, trascinare il mondo. E come si sceglie una locomozione che non faccia stare male.

## Come si parte
1. Unity chiuso, nella tua cartella del corso: `git aggiorna`. Questa volta lancialo due volte: il secondo giro rimette come in origine anche gli Starter Assets di XR Interaction Toolkit.
2. Apri il progetto `sandbox/` con Unity Hub.
3. **RUFA ▸ Tutorial**, poi scegli `Lezione 02 - Locomozione` dal menu in alto a sinistra della finestra.
4. Premi **Importa la palestra**. Se hai ancora la palestra della volta scorsa, ti chiede di sostituirla: rispondi **Sostituisci** (la copia vecchia va nel Cestino).

Il tutorial ti guida pagina per pagina. Le pagine con **Da fare** vanno avanti solo quando il progetto è a posto. Con **A−** e **A+** cambi la dimensione del testo.

## La giornata
- **Insieme** (capitoli 0–6): lavori in una palestra con un rig sabotato. Lo provi, lo apri, lo curi; poi progetti il teleport e provi salto, scala a pioli, grab move e vignetta.
- **Da soli · Verifica**: un percorso a ostacoli dalla A alla B, da superare con quello che hai imparato. I compiti si spuntano da soli mentre lavori, con il punteggio in alto: è un'autovalutazione.
- **Da soli · Il tuo progetto**: crei la scena del tuo progetto d'esame, in `Assets/Progetto`, configuri il comfort del tuo rig e costruisci il primo tratto esplorabile.

## Due rig
In Play senza visore parte il **rig desktop** (WASD e mouse), che non usa il prefab XR: se cambi il prefab XR e premi Play, non vedi nessuna differenza. Per provare il rig XR senza visore accendi **RUFA ▸ Play mode ▸ Simulatore VR**; per provarlo nel visore usa **RUFA ▸ Play mode ▸ VR con Quest Link** oppure la build.

## Comandi del simulatore

| Azione | Tasti |
|---|---|
| Guardarsi intorno | tasto destro del mouse tenuto, poi il mouse |
| Camminare (levetta sinistra) | Shift + I J K L; lascia prima la lettera, poi Shift |
| Girare, teleport (levetta destra) | I J K L |
| Teleport arrivando girati a sinistra | tieni I, aggiungi J, lascia I e per ultimo J (a destra: L al posto di J) |
| Grip, grilletto, tasti A e B | G, T, 1, 2 (controller destro); con Shift, il sinistro |
| Muovere col mouse solo un controller | [ (sinistro), ] (destro); Tab torna a testa e mani insieme |
| Spostare la testa come camminando nella stanza | W A S D, Q E: la testa attraversa i muri e non cade |
| Salire la scala a pioli | avvicinati finché un piolo si colora, tieni G e poi lascialo: sei in cima (per salire a mano, tenendo G: ] e Q) |
| Volare (con Enable Fly acceso) | tasto destro tenuto e mouse per guardare dove vuoi andare, poi Shift + I; se lasci, cadi |
| Provare il grab move | ] (solo il controller destro), poi G tenuto e insieme A o D; Tab per tornare |

Se il rig cammina da solo, premi di nuovo Shift. Se hai spostato la testa con WASD, esci e rientra in Play.

## Comandi del Quest

| Azione | Comando |
|---|---|
| Camminare | Stick sinistro |
| Snap turn | Stick destro a sinistra o a destra |
| Mezzo giro | Stick destro indietro |
| Teleport | Stick destro in avanti: compare l'arco, rilascia per teletrasportarti (il grip annulla) |
| Saltare | Tasto A |
| Arrampicarsi | Grip su un piolo, poi tira verso il basso |
| Afferrare | Grip |
| Usare, premere la UI | Trigger |

## Valori di comfort

| Parametro | Valore |
|---|---|
| Velocità (Move Speed) | 1,5–2,5 m/s |
| Rotazione | snap turn di 30° o 45°, mai continua |
| Step Offset | 0,2 m |
| Teleport | Teleportation Area solo dove si sta in piedi, layer Teleport |
| Vignetta | `TunnelingVignette` sotto la camera, su Move e Turn |

## Problemi frequenti

| Sintomo | Causa | Rimedio |
|---|---|---|
| Attraversi i muri e non cadi | ti muovi con WASD, che sposta la testa | cammina con Shift + I J K L |
| Cambio il prefab XR ma in Play non cambia niente | sta girando il rig desktop | **RUFA ▸ Play mode ▸ Simulatore VR** |
| I valori tornano come prima | li hai cambiati in Play, sulla copia `(Clone)` | cambiali in Prefab Mode e salva |
| Il teleport non parte su una superficie | manca la Teleportation Area, o non è sul layer Teleport | Add Component ▸ Teleportation Area, Interaction Layer Mask = Teleport |
| Sull'anchor arrivo girato male | la freccia blu dell'anchor, l'asse Z, non punta verso la stazione | ruota l'anchor finché la freccia blu non guarda la stazione |
| Il tasto del salto non fa niente | Jump Input senza azione | `XRI Right Locomotion/Jump` nei due campi di Jump Input |

## Consegna
Il lavoro, salvato a Unity chiuso con `git salva "Lezione 02"`.
