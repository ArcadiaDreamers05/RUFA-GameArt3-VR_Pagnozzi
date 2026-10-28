# Lezione 4 — Level design e blockout

## Cosa impari
Come si progetta uno spazio in VR prima di arredarlo: le misure in metri e quelle di casa, il corpo del rig (perché una porta bassa ferma chi cammina e non chi si teletrasporta), l'area di gioco, la testa che entra nei muri, scale e rampe, i modelli da Blender e Maya alla scala giusta e coi collider, il teleport, gli anchor e il punto di partenza, come guidare lo sguardo con la luce, anche nei corridoi bui, e come si costruisce un panorama. E come si scrive e si presenta il concept del tuo progetto: è la prima milestone del corso.

## Come si parte
1. Unity chiuso, nella tua cartella del corso: `git aggiorna`.
2. Apri il progetto `sandbox/` con Unity Hub.
3. **RUFA ▸ Tutorial**, poi scegli `Lezione 04 - Level design` dal menu in alto a sinistra della finestra.
4. Premi **Importa la palestra**: si apre la Casa storta.

Il tutorial ti guida pagina per pagina. Le pagine con **Da fare** vanno avanti solo quando il progetto è a posto. Con **A−** e **A+** cambi la dimensione del testo.

## La giornata
- **Insieme** (capitoli 0–2): la camminata critica nella Casa storta, una casetta di cubi con undici difetti di spazio, uno per postazione; poi li curi uno alla volta.
- **Il concept** (capitolo 3): lo scrivi dal modello e lo presenti al docente quando ti chiama, 3 minuti.
- **Da soli · Verifica**: il Rifugio, una pianta disegnata a terra da costruire a misura. I compiti si spuntano da soli mentre lavori: è un'autovalutazione.
- **Da soli · Il tuo progetto**: il blockout del tuo livello.

## Le misure
In Unity **1 unità = 1 metro**, e nel visore lo senti col corpo. La Scale di un cubo è la sua misura in metri: un muro lungo 4 m, alto 3 e spesso 20 cm ha Scale 4, 3, 0,2. Per le stanze valgono le misure del mondo reale:

| Cosa | Misura |
|---|---|
| Porta | 80–90 × 210 cm |
| Soffitto | 2,7–3,0 m |
| Corridoio | almeno 1,2 m |
| Gradino | alzata 16–18 cm, pedata 28–30 cm |
| Oggetto da prendere | tra 0,7 e 1,3 m da terra |
| Tavolo, scrivania | 72–76 cm |
| Sedia (seduta) | 45 cm |
| Maniglia, interruttore | 1,0–1,1 m |

L'altezza della testa la mette il visore: il rig nasce coi piedi dove sta il `Rig Bootstrap`, e la testa sta all'altezza vera di chi gioca. Se nel visore ti senti troppo alto o troppo basso, ricalibra il pavimento nel Quest, non spostare la camera. Il manichino da 1,75 m e la porta da 80 × 210 cm del corso sono i riferimenti: tienili accanto a quello che costruisci.

Per costruire a misura: tenendo **Ctrl** mentre trascini, l'oggetto si sposta a passi fissi (il passo si regola nel menu della griglia, in alto nella Scene view: 0,1, cioè 10 cm); tenendo **V** prendi un vertice e lo appoggi su quello di un altro oggetto. Quando conosci i valori, scrivili nell'Inspector.

## Cosa dicono Meta e XR Interaction Toolkit
- **Il corpo del rig.** Il rig ha un corpo: il Character Controller, una capsula alta quanto la tua testa, che XR Interaction Toolkit aggiorna prima di ogni passo. Camminando, un architrave più basso della testa ti ferma; il teleport invece porta il rig a destinazione senza guardare la capsula. Prova le porte con chi è più alto di te.
- **Scale e rampe.** Meta consiglia rampe dolci invece delle scale, pochi gradini, e il teleport per i dislivelli: col movimento continuo ogni gradino è uno scatto della testa. Nel Character Controller, **Step Offset** è il gradino più alto che si sale camminando (il manuale di Unity consiglia da 10 a 40 cm) e **Slope Limit** la pendenza più ripida. Il trucco del mestiere: una rampa invisibile, un cubo inclinato senza Mesh Renderer, appoggiata sugli spigoli dei gradini.
- **Spazi aperti.** Meta consiglia spazi aperti e di non far camminare chi gioca rasente ai muri, dove la parete che scorre vicina dà più fastidio.
- **La luce guida.** L'occhio va dove c'è luce: Meta fa l'esempio della porta che si illumina perché tu la apra. I cambi di luce bruschi invece danno fastidio.
- **Il dettaglio dove vuoi lo sguardo.** Lo sguardo va dove c'è più dettaglio; un fondo troppo ricco distrae.
- **Le cose importanti davanti.** Quello che conta sta nel campo visivo, non di lato o alle spalle, dove costringe a girare tutta la testa.
- **I testi nel mondo** almeno a mezzo metro, comodi intorno a un metro.
- **Il teleport** arriva solo su una Teleportation Area del layer **Teleport**. Su un'area che ha anche le pareti, **Filter Selection By Hit Normal** evita che puntare la parete porti in cima.
- **La partenza.** Il rig nasce dove sta il `Rig Bootstrap`, rivolto come lui: giralo verso la prima cosa da vedere.

Dalla pratica del level design: un landmark per stanza, un oggetto grande e riconoscibile che dice dove sei; compressione e apertura, cioè un passaggio stretto e poi una stanza grande; in VR dieci metri di corridoio vuoto sono lunghi, e nel walking simulator serve un motivo di interesse ogni 5–8 m.

## L'area di gioco
Nel visore chi gioca si muove anche col corpo vero, dentro il confine della sua stanza. Meta chiede di progettare per 2 × 2 m in piedi e per 1 × 1 m da fermi, e circa metà dei giocatori sta seduta: da seduti nessuna azione deve chiedere di alzarsi, di chinarsi fino a terra o di girarsi oltre 90°. Ogni enigma sta dentro 2 × 2 m, con quello che serve a portata di mano; niente da raccogliere da terra; gli enigmi davanti, non alle spalle. **Aggiungi i riferimenti di scala** disegna il quadrato di 2 × 2 m a terra, intorno alla partenza.

## La testa nei muri
La capsula del rig ferma i passi fatti con la levetta, non la testa: chi gioca può sempre sporgersi, e la testa entra nei muri. Meta chiede di non spostare né bloccare la camera, ma di spegnere la vista: chi gioca vede nero e si tira indietro. XR Interaction Toolkit non lo fa: nel corso c'è il componente **Head Blackout**, da mettere alla radice del rig del progetto, `XR Origin (progetto)`. Spegne la vista quando la testa è a meno di **Radius** (0,1 m) da un muro, e anche quando è passata del tutto oltre una parete sottile, finché non torna dalla sua parte. Quello che tieni in mano non conta. Per l'escape room: muri spessi, e niente indizi subito dietro una parete sottile.

## Gli anchor
I punti d'arrivo del teleport, che Meta chiama hotspot, sono i **Teleportation Anchor** della lezione 2:
- davanti a ogni enigma, rivolti all'enigma, con **Match Orientation** Target Up And Forward;
- almeno a mezzo metro dai muri: arrivare col naso sulla parete è il modo più facile di sbirciare;
- in cima e in fondo a ogni scala.

Da **GameObject ▸ XR ▸ Teleportation Anchor** nasce un piano di 10 × 10 m: portalo a **Scale** (0,1, 1, 0,1), e in **Interaction Layer Mask** lascia solo **Teleport**.

## La luce nei corridoi bui
Nei corridoi bui non si illumina il tratto dritto, ma la svolta: la parete che hai davanti quando giri si accende e dice "di qua", e il resto resta in penombra. È un trucco degli horror, come Resident Evil 2 e Left 4 Dead. Una luce sola tira, troppe confondono; alla fine, una luce sull'uscita, la porta che si illumina. Sul Quest 3S, che ha uno schermo LCD, quello che è più scuro di 13 su 255 diventa nero pieno: fai le zone buie un po' più chiare di come le vedi sul monitor.

## Il panorama
Dopo un passaggio buio o stretto, una vista che si apre: è la compressione e apertura nella sua forma più forte. Una vista si costruisce in tre piani: vicino una cornice (un arco, un parapetto, una finestra), in mezzo riferimenti di misura nota (case, alberi, una strada), lontano un richiamo (una torre, le montagne). Senza i riferimenti in mezzo, nel visore una montagna sembra un fondale. La profondità la dà la nebbia, che schiarisce quello che è lontano. In **Window ▸ Rendering ▸ Lighting**, scheda **Environment**:
- **Skybox Material**: un cielo senza terreno, per esempio da Poly Haven (CC0);
- **Fog** acceso, **Mode** Linear, **Start** ed **End** secondo le distanze del tuo livello (nella Casa storta 50 e 1000 m: la camera del rig vede fino a 1000 m), e il **Color** preso col contagocce sull'orizzonte del cielo.

Il paesaggio lontano fallo con forme semplici: da lontano i dettagli non si vedono.

## Modelli da Blender e Maya
Seleziona il modello nella finestra Project: nell'Inspector, scheda **Model**:
- **Scale Factor** moltiplica tutto il modello;
- **Convert Units** porta in metri l'unità del file: Maya lavora in centimetri, e con Convert Units acceso 210 cm diventano 2,1 m;
- **Bake Axis Conversion**: in Blender l'alto è l'asse Z, in Unity è la Y, e un modello di Blender arriva in scena con la Rotation X a −90 per stare dritto. Acceso, Unity raddrizza il modello una volta per tutte, e in scena la sua Rotation è 0;
- **Generate Colliders** dà al modello i suoi Mesh Collider: senza, il corpo e il teleport ci passano attraverso.

Dopo ogni cambio premi **Apply**. La prova più semplice, da fare una volta per programma: esporta un cubo da 1 m e mettilo accanto a un cubo di Unity. Esporta sempre in FBX: i file .ma e .mb di Maya Unity li apre con regole sue. Se il modello arriva 100 volte troppo grande o troppo piccolo, è l'unità del file: Convert Units o Scale Factor.

## Il blockout
Il blockout è il livello fatto di volumi semplici, senza grafica: serve a provare misure, percorsi e tempi prima dei modelli veri. Coi cubi (**GameObject ▸ 3D Object ▸ Cube**), in Blender o in Maya.
- Muri spessi 20 cm, alle misure di casa; ogni oggetto col nome della sua stanza, per esempio `Ingresso - muro nord`.
- I modelli importati con Convert Units e Generate Colliders, e la prova del cubo da 1 m.
- Una Teleportation Area su ogni pavimento, layer Teleport, e il `Rig Bootstrap` dove si parte, rivolto alla prima cosa da vedere.
- Una luce provvisoria dove vuoi portare lo sguardo.
- I riferimenti di scala in scena: nel tutorial, **Aggiungi i riferimenti di scala** mette il manichino, la porta e il quadrato dell'area di gioco davanti al punto di partenza.
- Percorribile dall'inizio alla fine, e provato nel visore: con **RUFA ▸ Play mode ▸ VR con Quest Link** o con una build. La scala si giudica solo lì.
- Un anchor davanti a ogni enigma, a mezzo metro dai muri; ogni enigma dentro 2 × 2 m e giocabile da seduti.
- **Head Blackout** sul rig del progetto; muri spessi, niente indizi dietro pareti sottili.
- Ringhiere dove si può cadere, e un pavimento sotto tutto: nessuno deve restare bloccato fuori dalla mappa.
- I tastierini vicino ai muri: sul loro **Tracked Device Graphic Raycaster** accendi **Check For 3D Occlusion**, se no il raggio li clicca attraverso le pareti.
- Gli indizi non solo col colore: aggiungi una forma o una scritta.
- Una rampa invisibile blocca anche l'arco del teleport: se ci si deve arrivare col teleport, dalle una Teleportation Area.

## Il concept e il pitch
Nel tutorial, **Crea il concept** scrive `Consegne/L04/concept.md` dal modello e lo apre (se Windows chiede con quale programma, scegli Blocco note). Sostituisci ogni «da scrivere»:
1. **Titolo e genere**: walking simulator o escape room.
2. **Ambientazione e tono**, in due righe: cosa prova chi gioca.
3. **Struttura**: per il walking simulator i 3–5 momenti in ordine, ognuno con cosa si vede, cosa si sente e il trigger che lo avvia; per l'escape room la catena degli enigmi, 3–5 passaggi "trovo X → uso X su Y → si apre Z", coi mattoncini della lezione 3.
4. **Pianta** in scala, col percorso e i punti di interesse numerati: a mano o in qualunque programma, salvata come `pianta.png` accanto al concept.
5. **Perimetro**: stanze e misure, asset (solo CC0: Poly Haven, ambientCG, Kenney, The Base Mesh) e da dove li prendi, cosa tagli se manca il tempo.

Il pitch dura 3 minuti, col concept aperto. Il docente guarda quattro cose: è fattibile in 11 settimane, da solo (2–3 stanze per l'escape room, 3–5 momenti per il walking simulator); si costruisce coi mattoncini della lezione 3, senza programmazione nuova; gli asset sono CC0 e sai da dove vengono; il percorso è chiaro, si capisce dove si parte, dove si va e quando è finita. Esce approvato, approvato con modifiche (le scrivi nel concept) o da rifare entro la lezione 5. Senza approvazione non si va avanti.

## Comandi del simulatore
Come nella lezione 2: **Shift + I J K L** è la levetta sinistra, per camminare; **I J K L** la destra, per girare a scatti e per il teleport. Per il teleport tieni **I**, punta l'arco col mouse (tasto destro tenuto) e lascia **I**: l'arco è blu dove puoi arrivare, rosso dove no. Col tasto destro del mouse tenuto giri anche lo sguardo. Il simulatore parte in modalità FPS, dove **W A S D** spostano la testa e le mani come un passo vero, senza la levetta: è la prova della testa nei muri. Tienili premuti un secondo, non di più: la testa passa anche i muri esterni, e il corpo la segue. Se W non muove la testa, premi **Tab**, che entra ed esce dalla modalità FPS.

## Problemi frequenti

| Sintomo | Causa | Rimedio |
|---|---|---|
| Il compito resta ✗ ma il muro c'è | il modello importato non ha collider | Generate Colliders, Apply |
| La porta non passa la misura | un cubo pieno sopra il varco, o la porta non è dove la pianta la segna | due muri e un architrave, sulla linea della pianta |
| La rampa non passa | troppo ripida (oltre circa 30°) o non arriva al bordo della pedana | allungala fino al bordo |
| Il teleport non va | la Teleportation Area non ha il layer Teleport | Interaction Layer Mask: Teleport |
| L'ambiente intatto resta ✗ | un pezzo di `Ambiente` spostato | Ctrl+Z |
| Camminando ti fermi sotto una porta | l'architrave è più basso della tua testa | portalo a 2,1 m |
| Nel visore tutto sembra enorme o minuscolo | il modello è importato alla scala sbagliata | Convert Units o Scale Factor; la prova del cubo da 1 m |
| Ci si sente troppo alti o troppo bassi | il pavimento del Quest è calibrato male | ricalibra il pavimento nel Quest, non spostare la camera |
| Si parte guardando un muro | il `Rig Bootstrap` è girato male | ruotalo verso la prima cosa da vedere |
| I valori tornano come prima | li hai cambiati in Play | cambiali a gioco fermo e salva |
| Vicino a un muro la vista diventa nera | è Head Blackout: la testa è a meno di 10 cm da un muro, o ci è entrata | tirati indietro |
| La verifica della testa nel muro resta ✗ | Head Blackout aggiunto al rig in scena o in Play | alla radice del prefab `XR Origin (lezione 4)`, a gioco fermo, e salva |
| Il panorama resta ✗ | la nebbia non è Linear, o Skybox e nebbia cambiati in Play | Mode Linear, a gioco fermo, e salva |
| L'anchor non ti gira verso l'enigma | Match Orientation su World Space Up | Target Up And Forward |
| Dopo l'anchor soffitto e rampa del Rifugio non passano più | l'anchor è ancora il piano di 10 × 10 m | Scale (0,1, 1, 0,1) |
| Il raggio clicca un tastierino attraverso il muro | Check For 3D Occlusion spento | accendilo sul Tracked Device Graphic Raycaster |
| Nel visore la stanza buia è nera pieno | l'LCD del Quest 3S sotto 13 su 255 non mostra niente | schiarisci un po' le zone buie |

## Consegna
In `Consegne/L04/` del tuo repository: `concept.md`, dal pulsante del tutorial, e la pianta, `pianta.png`. Il blockout nella scena `Progetto`, percorribile dall'inizio alla fine coi riferimenti di scala ancora in scena: lo guarda il docente. A fine lezione, a Unity chiuso: `git salva "Lezione 04"`.
