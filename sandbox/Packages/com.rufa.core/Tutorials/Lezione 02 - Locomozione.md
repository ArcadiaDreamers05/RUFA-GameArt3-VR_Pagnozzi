Tutorial della lezione 2, letto da RUFA > Tutorial e da tools/stima-tempi.py. Sintassi: "# " capitolo ("# Da soli ..." = lista di compiti, in qualsiasi ordine; un compito "## Extra..." è un punto in più, fuori dal punteggio), "## " pagina; righe "> evidenzia: Finestra | Etichetta | Oggetto (facoltativo: il campo si evidenzia solo su quell'oggetto)", "> verifica: id arg | arg", "> azione: Testo del pulsante | id | arg", "> attesa: secondi cosa", "> lavoro: minuti-lento minuti-veloce".

# 0 · Si parte

## Cosa farai in questa lezione
Oggi impari la locomozione di XR Interaction Toolkit, cioè tutti i modi di far muovere il giocatore in VR. La prima parte la facciamo insieme, passo passo: lavori in una palestra, un percorso con una scala, una rampa e un soppalco, e un rig VR che qualcuno ha sabotato. Prima lo provi e trovi sei difetti, poi lo apri e capisci com'è fatto, poi lo curi; infine progetti il teleport e provi gli altri modi di muoversi.

Poi lavori da solo: una verifica, cioè un percorso a ostacoli da superare con quello che hai imparato, e il primo pezzo del tuo progetto d'esame.

Questa finestra ti accompagna. Le pagine con **Da fare** in basso vanno avanti solo quando il progetto è davvero a posto: se **Avanti** resta grigio, c'è ancora qualcosa da sistemare. Le pagine senza scritta sono da leggere.

Con **A−** e **A+** cambi la dimensione del testo; dal menu sotto la barra salti a un capitolo.

## Importa la palestra
Premi il pulsante qui sotto. Unity copia la palestra nel tuo progetto, in `Assets/Samples/RUFA Core/Lezione 02 - Locomozione`, e apre la scena `Percorso`. Se nel progetto c'è una palestra vecchia, prima ti chiede di sostituirla: rispondi **Sostituisci**.

Quella copia è tua: puoi modificarla quanto vuoi, `git aggiorna` non la tocca e premere di nuovo il pulsante non la sovrascrive. Se un giorno vuoi ripartire da zero, cancella la cartella `Lezione 02 - Locomozione` e premi di nuovo.
> azione: Importa la palestra | importa-sample | Lezione 02 - Locomozione
> verifica: sample-importato Lezione 02 - Locomozione

## Due rig, due mondi
Il progetto ha due rig, cioè due modi di mettere il giocatore nella scena, e all'avvio il `Rig Bootstrap` sceglie quello giusto:
- sul Quest, con Quest Link e con il simulatore parte il **rig XR**: il prefab `XR Origin` degli Starter Assets di XR Interaction Toolkit, con visore, controller e locomozione;
- in Play senza visore, e nella build per PC, parte il **rig desktop**: lo costruisce il codice del progetto, con WASD, mouse e mirino.

Il rig desktop non legge niente dal prefab XR. Se cambi la velocità nel prefab e premi Play senza visore, non succede niente: non stai usando quel rig. Per ricordartelo, in Play senza visore compare per qualche secondo un avviso in alto a sinistra.

È il **doppio target** del progetto: un solo progetto, due piattaforme. Oggi lavoriamo sul rig XR.

Per andare avanti, la scena aperta deve essere `Percorso`.
> verifica: scena-aperta Percorso

## Accendi il simulatore
Per provare il rig XR senza visore usi il simulatore di XR Interaction Toolkit: **RUFA ▸ Play mode ▸ Simulatore VR**. La prima volta Unity lo importa nel progetto, e ci vuole qualche minuto.

In Play il simulatore si comanda con tastiera e mouse:
- tasto destro del mouse tenuto premuto, poi il mouse: ti guardi intorno, e le mani seguono lo sguardo;
- **Shift + I J K L**: la levetta sinistra, per camminare;
- **I J K L**: la levetta destra, per girare e per il teleport;
- **G** il grip, **T** il grilletto, **1** e **2** i tasti del controller destro; con **Shift**, quelli del sinistro;
- **[** e **]**: il mouse muove solo il controller sinistro o solo il destro; **Tab** torna a muovere testa e mani insieme;
- **W A S D** e **Q E** spostano la testa come se camminassi davvero nella stanza: la testa attraversa i muri e non cade, perché il corpo del rig resta fermo. Per muoverti nel gioco usa le levette; se hai spostato la testa, esci e rientra in Play.

Con Shift e una lettera, lascia prima la lettera e poi Shift: se lasci prima Shift, la levetta sinistra resta spinta e il rig cammina da solo. Per fermarlo, premi di nuovo Shift.
> verifica: simulatore-acceso

## Entra in Play
Premi **Play**. Davanti a te c'è un muro con un varco. Oltre il varco, a sinistra, una scala sale a un pianerottolo e una rampa riscende; a destra c'è un gradino alto 40 cm. Più avanti, sempre a destra, un soppalco a 2,5 m; in fondo un tavolo e una porta: le due stazioni d'enigma.

Resta in Play: nel prossimo capitolo provi il rig.
> verifica: in-play
> attesa: 15 entrata in Play

# 1 · Il malato

## Come si fa una diagnosi
Un rig che fa stare male si cura come un paziente: prima i sintomi, poi la causa. Prova una cosa alla volta, descrivi a parole cosa succede, e solo dopo cerca il responsabile.

In Play il rig nella Hierarchy si chiama `XR Origin (sabotato)(Clone)`. I modi di muoversi stanno sotto `Locomotion`, i controller sotto `Camera Offset`.

In ogni pagina di questo capitolo, quando pensi di aver trovato l'oggetto responsabile, **selezionalo nella Hierarchy**: se è quello giusto, la pagina diventa **Fatto**.

Non correggere ancora niente: in Play le modifiche si perdono. La cura arriva nel capitolo 3.

## Sintomo 1: si corre
Tieni premuti **Shift** e **I**: è la levetta sinistra spinta in avanti. Attraversi il percorso di corsa: il rig va a 6 m/s, quattro volte un passo svelto. Dieci metri in meno di due secondi.

In VR una velocità così è una delle cause di malessere più forti: gli occhi vedono un movimento rapido che il corpo, fermo, non sente.

Quale oggetto decide la velocità del movimento continuo?
> verifica: selezionato Move

## Sintomo 2: si gira senza sosta
Spingi la levetta destra di lato (**J** o **L**). Invece di girare a scatti, la visuale ruota in continuo: 120 gradi al secondo, mezzo giro in un secondo e mezzo.

La rotazione continua è la causa di nausea più forte in assoluto. Per questo quasi tutti i giochi VR girano a scatti di 30° o 45°: lo **snap turn**.

Che cosa decide che cosa fa la levetta destra? Cercalo tra i controller o tra i modi di girare.
> verifica: selezionato Right Controller | Turn

## Sintomo 3: si va dove punta la mano
Premi **[**: ora il mouse muove solo il controller sinistro. Tieni premuto il tasto destro del mouse e sposta il mouse di lato: il controller ruota. Poi tieni **Shift + I**: non vai dove guardi, vai dove punta la mano. Per tornare come prima: **]** e poi **Tab**.

Muoversi seguendo la mano ha senso solo in casi particolari. Per camminare in un ambiente, la direzione la deve decidere lo sguardo.

Quale oggetto decide la direzione del movimento?
> verifica: selezionato Move

## Sintomo 4: si galleggia
Sali la scala fino al pianerottolo (**Shift + I**) e prosegui oltre il bordo, oppure scendi dalla rampa: resti sospeso a mezz'aria. Nessuno ti tira giù.

Quale oggetto applica la gravità al giocatore?
> verifica: selezionato Gravity

## Sintomo 5: il gradino da 40 cm
Cammina (**Shift + I**) verso il gradino alto 40 cm, a destra dopo il varco. Ci sali sopra come se non ci fosse. Nessuno sale 40 cm senza alzare il ginocchio, e il corpo del giocatore lo sa.

Il limite all'altezza che si supera camminando sta nel componente che dà un corpo fisico al rig, sulla sua radice. Seleziona la radice del rig.
> verifica: selezionato XR Origin (sabotato)(Clone) | XR Origin (sabotato)

## Sintomo 6: ci si teletrasporta sui muri
Spingi in avanti la levetta destra e tienila (**I**, senza Shift): compare l'arco del teleport. Puntalo col mouse (tasto destro tenuto) sul muro del varco o sul tavolo: l'arco è valido, e quando rilasci ti ritrovi lì sopra.

Il teleport deve funzionare solo dove si può stare in piedi. Quale oggetto della scena accetta il teleport per sbaglio?
> verifica: selezionato Muro | Stazione - Tavolo

## Esci dal Play
Hai trovato i sei difetti: la velocità, la rotazione continua, la direzione della mano, la gravità, il gradino, il teleport sui muri.

Esci dal Play: la cura si fa a gioco fermo.
> verifica: fuori-play

# 2 · Anatomia del rig

## Apri il prefab
Nella cartella della palestra fai doppio clic su `XR Origin (sabotato)`: si apre in **Prefab Mode**, con la Hierarchy del solo rig.

È una **Prefab Variant** del rig degli Starter Assets: eredita tutto dall'originale, tranne i valori che cambia. Nell'Inspector quei valori sono in grassetto, con una barra blu a sinistra: sono gli **override**, cioè il sabotaggio. Il menu **Overrides**, in alto, li elenca tutti.

Le varianti servono proprio a questo: cambiare poche cose di un prefab senza copiarlo.
> verifica: prefab-aperto XR Origin (sabotato)

## XR Origin: il punto zero del giocatore
Seleziona la radice, `XR Origin (sabotato)`. Il componente **XR Origin** dice dove sta il giocatore nella scena:
- **Origin Base GameObject**: l'oggetto che si sposta quando ti muovi, cioè la radice;
- **Camera Floor Offset GameObject**: `Camera Offset`, che porta camera e controller all'altezza giusta;
- **Camera**: `Main Camera`, i tuoi occhi;
- **Tracking Origin Mode**: Not Specified, Device, Floor o Unbounded. Con Floor l'altezza zero è il pavimento del Guardian del Quest; con Device l'altezza la decide il campo qui sotto; Unbounded serve agli spazi molto grandi. Qui è Not Specified, e il Quest usa il pavimento;
- **Camera Y Offset**: 1,36 m, l'altezza degli occhi usata solo con Device, per esempio per giocare da seduti.
> evidenzia: Inspector | Tracking Origin Mode
> verifica: selezionato XR Origin (sabotato)

## Il tracking: chi muove testa e mani
Seleziona `Main Camera`. Il componente **Tracked Pose Driver (Input System)** legge a ogni fotogramma la posizione e la rotazione del visore e le copia sulla camera: sono i **6 gradi di libertà**, tre di spostamento e tre di rotazione.

`Left Controller` e `Right Controller` fanno lo stesso con i due controller. Tutti e tre sono figli di `Camera Offset`, che li porta all'altezza giusta rispetto al pavimento.

Il tracking non è locomozione. Se cammini davvero nella stanza, il visore si sposta dentro il rig; se spingi la levetta, è il rig intero a spostarsi nella scena.
> evidenzia: Inspector | Tracked Pose Driver (Input System) | Main Camera
> verifica: selezionato Main Camera

## L'input dei controller
Seleziona la radice: il componente **Input Action Manager** attiva l'asset `XRI Default Input Actions`. Lì le azioni di XR Interaction Toolkit sono divise in mappe, per esempio `XRI Left Locomotion` e `XRI Right Locomotion`. Un'azione collega un comando del controller, come la levetta sinistra, a un nome, come Move.

Ogni provider legge l'input attraverso un **input reader**: il campo `Left Hand Move Input` del Move, per esempio, dice da quale azione arriva la spinta della levetta sinistra.

Ora seleziona `Right Controller`. Il **Controller Input Action Manager** decide che cosa fa la levetta di quella mano:
- **Smooth Motion Enabled**: la levetta muove in continuo, invece di fare il teleport;
- **Smooth Turn Enabled**: la levetta ruota in continuo, invece che a scatti.

Di solito a sinistra è acceso il primo e a destra sono spenti tutti e due: levetta sinistra per camminare, levetta destra per girare a scatti e per il teleport.
> evidenzia: Inspector | Smooth Turn Enabled | Right Controller
> verifica: selezionato Right Controller

## Locomotion: il mediatore e il corpo
Seleziona `Locomotion`. Ci sono due componenti che coordinano tutti i modi di muoversi:
- il **Locomotion Mediator** fa muovere un provider alla volta: mentre ti teletrasporti, la levetta non ti fa anche camminare;
- l'**XR Body Transformer** applica il movimento all'XR Origin. Se sulla radice c'è un Character Controller, lo usa per muovere il corpo, che così si scontra con muri e gradini.

Ogni provider passa per quattro stati: **Idle**, fermo; **Preparing**, si prepara a muovere; **Moving**, sta muovendo il corpo; **Ended**, ha finito. Il mediatore lascia il corpo a un solo provider alla volta.

In XR Interaction Toolkit 3 la struttura è questa: un mediatore, un corpo e tanti provider figli. Chi programma può aggiungere un modo di muoversi suo: un provider che chiede al Body Transformer di spostare il corpo, l'**XR Movable Body**, con una trasformazione, `IXRBodyTransformation`.
> verifica: selezionato Locomotion

## Il Character Controller
Seleziona la radice e trova il **Character Controller**: è il corpo fisico del giocatore, una capsula invisibile.
- **Height**: l'altezza della capsula; durante il gioco segue l'altezza della testa;
- **Radius**: 0,1 m, lo spessore;
- **Slope Limit**: 45°, la pendenza massima che si sale camminando;
- **Step Offset**: 0,5 m, il gradino più alto che si supera senza fermarsi;
- **Skin Width**: un piccolo margine che evita di incastrarsi nelle pareti.

Lo Step Offset è il valore di default degli Starter Assets, ed è troppo alto: nel percorso le alzate della scala sono di 17 cm e il gradino è di 40. Anche i valori di default vanno controllati.
> evidenzia: Inspector | Step Offset
> verifica: selezionato XR Origin (sabotato)

## I provider: uno per ogni modo di muoversi
Sotto `Locomotion` ogni figlio è un **provider**, cioè un modo di muoversi:
- `Move`, **Dynamic Move Provider**: il movimento continuo con la levetta (Move Speed, Enable Strafe, Enable Fly, direzione dalla testa o dalla mano);
- `Turn`, **Snap Turn Provider** (Turn Amount 45°, Debounce Time 0,5 s, Enable Turn Around: levetta indietro = mezzo giro) e **Continuous Turn Provider** (Turn Speed);
- `Teleportation`, **Teleportation Provider**: esegue le richieste che arrivano dalle aree e dagli anchor di teleport (Delay Time è l'attesa prima del salto);
- `Gravity`, **Gravity Provider**: tira giù il giocatore quando non tocca terra;
- `Jump`, **Jump Provider**: il salto, con il tasto A;
- `Climb`, **Climb Provider**: l'arrampicata sulle scale a pioli, che hanno un **Climb Interactable**;
- `Grab Move`, **Grab Move Provider** e **Two-Handed Grab Move Provider**: ci si sposta trascinando il mondo con i grip; è spento di default.

Ognuno lavora da solo; il Locomotion Mediator decide chi muove il corpo in ogni momento.

## Componenti Legacy: da riconoscere, non da usare
Nei tutorial online troverai spesso componenti come **Locomotion System**, **Character Controller Driver**, o provider con il suffisso "(Action-based)" e "(Device-based)". Vengono da XR Interaction Toolkit 2: per i progetti vecchi ci sono ancora, in **Add Component ▸ XR ▸ Locomotion ▸ Legacy**.

In XR Interaction Toolkit 3 il loro lavoro lo fanno il Locomotion Mediator e l'XR Body Transformer. Se un tutorial te li fa aggiungere, è scritto per una versione precedente: cerca quella aggiornata, oppure traduci i passi sui componenti nuovi.

# 3 · La cura

## Si cura il prefab, non la copia in Play
Le cure si fanno qui, in Prefab Mode, sul prefab `XR Origin (sabotato)`. Ogni modifica va salvata: **Ctrl+S**, oppure lascia acceso **Auto Save** in alto a destra nella Scene view.

Quello che cambi in Play, sulla copia `(Clone)`, sparisce quando premi Stop: è l'errore più frequente. Le pagine di questo capitolo controllano il prefab salvato: se cambi un valore solo in Play, la pagina resta **Da fare**.
> verifica: prefab-aperto XR Origin (sabotato)

## Velocità
Seleziona `Locomotion/Move` e, nel Dynamic Move Provider, porta **Move Speed** tra 1,5 e 2,5 m/s. Un passo svelto è circa 1,4 m/s: in VR conviene restare poco sopra.

Nel walking simulator la velocità decide il ritmo dell'esplorazione: più è lenta, più si guarda.
> evidenzia: Inspector | Move Speed
> verifica: prefab-numero XR Origin (sabotato) | Locomotion/Move | DynamicMoveProvider | m_MoveSpeed | 1,5 | 2,5

## Rotazione a scatti
Seleziona `Camera Offset/Right Controller` e, nel Controller Input Action Manager, spegni **Smooth Turn Enabled**. La levetta destra torna a girare a scatti di 45°, il Turn Amount dello Snap Turn Provider in `Locomotion/Turn`. I valori più comodi sono 30° e 45°.
> evidenzia: Inspector | Smooth Turn Enabled | Right Controller
> verifica: prefab-vero XR Origin (sabotato) | Camera Offset/Right Controller | ControllerInputActionManager | m_SmoothTurnEnabled | no

## La direzione la decide la testa
Torna su `Locomotion/Move` e metti **Left Hand Movement Direction** su **Head Relative**: la levetta sinistra ti porta dove guardi.
> evidenzia: Inspector | Left Hand Movement Direction
> verifica: prefab-numero XR Origin (sabotato) | Locomotion/Move | DynamicMoveProvider | m_LeftHandMovementDirection | 0 | 0

## Gravità
Seleziona `Locomotion/Gravity` e accendi **Use Gravity** nel **Gravity Provider**. Non confonderlo con lo **Use Gravity** del Dynamic Move Provider di `Move`, già acceso: è un campo delle versioni vecchie di XR Interaction Toolkit e va lasciato com'è. Senza gravità il giocatore resta all'altezza a cui si trova: in un ambiente con scale e gradini è sempre un difetto.

Nel Dynamic Move Provider di `Move` c'è anche **Enable Fly**, la modalità volo. Con la spunta, mentre spingi la levetta vai dove guardi, anche in alto e in basso, perché la gravità resta sospesa; quando la lasci, la gravità ti riporta giù. Serve per un editor di livelli o per sorvolare una mappa, quasi mai in un gioco a piedi: qui resta spenta.
> evidenzia: Inspector | Use Gravity | Gravity
> verifica: prefab-vero XR Origin (sabotato) | Locomotion/Gravity | GravityProvider | m_UseGravity | sì

## Il gradino
Seleziona la radice e porta lo **Step Offset** del Character Controller a 0,2. Con 20 cm la scala da 17 cm si sale e il gradino da 40 no, come nella realtà.

Perché non 0,3, a metà strada? La capsula del corpo non tocca il pavimento: resta sollevata di qualche centimetro, la **Skin Width**, e con 0,3 a volte sale anche sui 40 cm. Uno Step Offset si sceglie sempre con un margine.
> evidenzia: Inspector | Step Offset
> verifica: prefab-numero XR Origin (sabotato) |  | CharacterController | m_StepOffset | 0,15 | 0,25

## I muri non sono pavimento
Esci dal prefab con la freccia in alto a sinistra nella Hierarchy e torna alla scena `Percorso`.

I due oggetti `Muro` e `Stazione - Tavolo` hanno un componente **Teleportation Area**, che dice al teleport "qui si può atterrare". Toglilo a tutti e tre: nell'Inspector, i tre puntini del componente, poi **Remove Component**. Poi salva la scena con **Ctrl+S**.

La Teleportation Area va messa solo dove il giocatore può stare in piedi: pavimenti, pianerottoli, il soppalco.
> verifica: nessun-teleport-su Muro | Stazione

## La prova del Percorso
Premi Play e fai il percorso d'un fiato con la levetta sinistra (**Shift + I**): il varco, la scala, il pianerottolo, la rampa, da scendere senza galleggiare. Poi prova il gradino da 40 cm: deve fermarti. Infine teletrasportati sul soppalco (**I**, mira, rilascia): è l'unico modo di salirci.

Quando sei sul soppalco, la pagina diventa **Fatto**. Se qualcosa non va, esci dal Play e torna alla pagina del difetto.
> verifica: sopra-oggetto Soppalco
> attesa: 15 entrata in Play
> lavoro: 4 2

# 4 · Il teleport progettato

## Area, Anchor e Multi-Anchor Volume
Il teleport di XR Interaction Toolkit ha tre tipi di destinazione:
- **Teleportation Area**: ovunque sulla superficie, come il pavimento;
- **Teleportation Anchor**: un punto preciso, con una direzione d'arrivo;
- **Teleportation Multi-Anchor Volume**: un volume con più punti d'arrivo; quando lo punti, il punto non lo scegli tu ma il volume, per default quello più lontano da te.

A eseguire il salto è il **Teleportation Provider** del rig. Nell'escape room gli anchor sono preziosi: portano il giocatore davanti all'enigma, già rivolto nella direzione giusta.

Esci dal Play, se ci sei ancora.
> verifica: fuori-play

## Gli Interaction Layer
Ogni interactor vede solo gli oggetti di certi **Interaction Layer**: l'interactor del teleport vede solo il layer `Teleport`, le mani solo `Default`.

Per questo i muri accettavano il teleport: la loro Teleportation Area era sul layer `Teleport`. E per questo un anchor nuovo va messo sul layer `Teleport`, altrimenti l'arco non lo vede.

Gli Interaction Layer non sono i layer di Unity che si usano per la fisica e per la camera: si scelgono nel campo **Interaction Layer Mask** di ogni interactor e di ogni interactable.

## Due anchor per le stazioni
Nella cartella della palestra c'è `Teleport Anchor`: trascinalo nella scena. È il Teleport Anchor degli Starter Assets, un disco con il componente **Teleportation Anchor**, più **XR Tint Interactable Visual**, che colora il disco quando l'arco lo punta. Mettilo a circa 1,2 m dal tavolo e ruotalo in modo che la freccia blu, l'asse Z, punti verso il tavolo: il figlio `Anchor` è il punto d'arrivo e guarda nella stessa direzione.

Nel componente Teleportation Anchor i due campi del teleport progettato sono già pronti:
- **Match Orientation**: Target Up And Forward, così arrivi rivolto come l'anchor;
- **Interaction Layer Mask**: solo `Teleport`, il layer che l'arco vede.

Lo stesso componente si crea anche da **GameObject ▸ XR ▸ Teleportation Anchor**, ma senza disco: un piano grigio che sul pavimento quasi non si vede.

Fai lo stesso davanti alla porta, `Stazione - Porta`, poi salva la scena.
> verifica: anchor-verso Stazione - Tavolo | Stazione - Porta
> lavoro: 6 3

## Prova gli anchor
Premi Play e punta un anchor: il disco si colora. Guarda anche il reticolo in fondo all'arco: è una freccia, e indica come sarai girato all'arrivo. Sul pavimento segue il tuo sguardo, perché lì arrivi girato come sei; sul disco di un anchor si gira verso la stazione.

Mettiti di fianco al tavolo, o di spalle, punta il disco e rilascia: arrivi al centro dell'anchor, già rivolto verso il tavolo. Poi teletrasportati sul pavimento lì accanto e confronta. È la differenza tra un'area, che ti porta dove punti, e un anchor, che ti porta in un punto e in una direzione decisi da chi ha progettato l'ambiente.

La pagina diventa **Fatto** quando arrivi su un anchor.
> verifica: su-anchor
> attesa: 15 entrata in Play
> lavoro: 4 2

## Quando parte il teleport
Esci dal Play e seleziona il `Pavimento`: nella Teleportation Area il campo **Teleport Trigger** dice in quale momento parte il salto:
- **On Select Exited**, il default: quando rilasci la levetta. Prima punti, poi decidi;
- **On Select Entered**: appena l'arco seleziona la destinazione;
- **On Activated** e **On Deactivated**: quando premi o rilasci il grilletto mentre punti.

Il default va bene quasi sempre. Il grilletto serve in un gioco in cui la levetta fa già altro.
> evidenzia: Inspector | Teleport Trigger | Pavimento
> verifica: selezionato Pavimento

## La linea dell'arco
Apri il prefab `XR Origin (sabotato)` e seleziona `Camera Offset/Right Controller/Teleport Interactor`. Nel componente **XR Ray Interactor**, **Line Type** decide la forma della linea:
- **Projectile Curve**, la curva balistica: sale e ricade, e raggiunge i posti più in alto di te;
- **Straight Line**: una retta, che non arriva dove non vedi;
- **Bezier Curve**: una curva più facile da controllare.

Il componente accanto, **XR Interactor Line Visual**, disegna l'arco: un colore quando la destinazione è valida, un altro quando non lo è, e in fondo il **reticolo**, la freccia che hai visto con gli anchor. Il campo **Blocked Reticle** contiene il reticolo che compare quando l'arco punta una destinazione di teleport che in quel punto non accetta il salto; dove l'arco non tocca nessuna destinazione, il reticolo sparisce.

Metti Line Type su **Straight Line**, salva, entra in Play e punta il pavimento davanti a te. La linea si piega a metà e finisce al centro del pavimento, e lì c'è anche il reticolo; ma se rilasci arrivi nel punto che stavi puntando. Il Line Visual piega le linee dritte verso il punto d'aggancio dell'oggetto selezionato: serve quando afferri un oggetto da lontano, e la linea lo segue. Il teleport però tiene selezionata l'area finché miri, e il punto d'aggancio di un'area è il suo centro. Con la curva non succede, perché la piega riguarda solo le linee dritte: per questo negli Starter Assets non si vede.

Esci dal Play e in **XR Interactor Line Visual** apri **Bending Enabled Interaction Layers** e togli la spunta a **Teleport**: la piega resta per gli oggetti da afferrare, non per le destinazioni del teleport. Salva, rientra in Play e punta di nuovo il pavimento: ora la linea va dritta dove punti, ed è lì che arrivi.
> evidenzia: Inspector | Line Type | Right Controller/Teleport Interactor
> verifica: prefab-numero XR Origin (sabotato) | Camera Offset/Right Controller/Teleport Interactor | XRInteractorLineVisual | m_BendingEnabledInteractionLayers.m_Bits | 0 | 2147483647
> lavoro: 6 3

## Il filtro per normale
Con la linea ancora dritta, in Play punta il soppalco da sotto: la linea lo prende lo stesso, sul fondo o sul fianco, e se rilasci ti ritrovi sopra. La Teleportation Area accetta ogni faccia del suo collider, anche quelle su cui non si sta in piedi.

Esci dal Play e dal prefab, seleziona il `Soppalco` e accendi **Filter Selection By Hit Normal** nella sua Teleportation Area: l'area scarta i punti in cui la superficie è inclinata più di **Up Normal Tolerance Degrees**, 30°, rispetto all'asse verticale dell'oggetto. Il fondo guarda in basso e i fianchi di lato: resta valida solo la superficie di sopra.

Rientra in Play e punta di nuovo il soppalco da sotto: la linea non trova più un punto valido, e non ci arrivi più. Una retta non arriva dove non vedi. Poi esci dal Play e salva la scena; nel prefab rimetti Line Type su **Projectile Curve**.

Su un muro con un'area il filtro scarta i lati, ma la sommità resta valida: è una rete di sicurezza, non sostituisce il togliere l'area dove non serve.
> evidenzia: Inspector | Filter Selection By Hit Normal | Soppalco
> verifica: scena-vero Soppalco | TeleportationArea | m_FilterSelectionByHitNormal | sì
> lavoro: 4 2

## Il Multi-Anchor Volume
Torna alla scena `Percorso`. Nella cartella della palestra c'è `Multi-Anchor Volume`: trascinalo nella scena e mettilo con **Position** 0 · 0 · 22,5, tra il tavolo e la porta. È una pedana azzurra, `Pedana`, con tre punti d'arrivo segnati da un disco: `Arrivo 1`, `Arrivo 2` e `Arrivo 3`. Sulla radice c'è il componente **Teleportation Multi-Anchor Volume**:
- **Anchor Transforms** elenca i tre punti d'arrivo;
- **Interaction Layer Mask** è solo `Teleport`, come per l'anchor;
- **Match Orientation** è Target Up And Forward: arrivi rivolto come la freccia blu del punto.

I punti sono fratelli della pedana, non suoi figli, così la sua scala non li schiaccia; e stanno sulla sua superficie, perché un arrivo dentro un collider incastra il giocatore.

Entra in Play e punta la pedana: non arrivi dove punti, ma nel punto che sceglie il volume, per default il più lontano da te. Spostati di lato, punta di nuovo e guarda come cambia. Poi esci dal Play, ruota di 90° il punto in cui sei arrivato e riprova: arrivi girato come lui. Il giocatore arriva sempre in un punto pensato da te.
> verifica: componente-in-scena TeleportationMultiAnchorVolume
> lavoro: 6 3

## I filtri del Multi-Anchor
Quale arrivo sceglie il volume lo decide il suo **filtro**, con una regola precisa. Nel componente apri **Destination Evaluation Settings** e poi **Constant Value**:
- **Destination Filter Object** è il filtro. Vuoto vuol dire **il più lontano da te**: il volume misura la distanza tra ogni arrivo e il punto del pavimento sotto la tua testa, e sceglie la più grande. L'idea è che punti una zona per andare avanti, e lui ti porta il più avanti possibile;
- la scelta si fa **quando l'arco entra nel volume**, e resta quella finché l'arco non esce. Con **Poll For Destination Change** il volume la rifà ogni **Destination Poll Frequency** secondi mentre punti; con **Enable Destination Evaluation Delay** la fa solo dopo che hai puntato il volume per **Destination Evaluation Delay Time** secondi.

Provalo con le torri d'osservazione. Nella cartella della palestra c'è `Torri di osservazione`: trascinalo nella scena con **Position** 0 · 0 · 28, in fondo al percorso. Il `Prato` è il Multi-Anchor Volume, la zona che punti. Le tre torri, a triangolo, sono i punti d'osservazione: i loro arrivi, `Cima 1`, `Cima 2` e `Cima 3`, sono in cima, rivolti verso il percorso.

Entra in Play, vai in fondo al percorso e punta il prato: arrivi in cima alla Torre 3, quella in fondo, perché è la più lontana da te. Scendi, punta il prato da un altro punto: finisci sempre sulla torre più lontana. È la regola di default.

Esci dal Play, seleziona `Prato` e trascina `Filtro sguardo`, dalla cartella della palestra, in Destination Filter Object; poi accendi Poll For Destination Change. Ora il volume sceglie l'arrivo **che stai guardando**: tra quelli che hai davanti, il più vicino al centro della tua vista. Rientra in Play, guarda una torre e punta il prato: arrivi in cima a quella.

Negli Starter Assets, in `DemoAssets/Settings`, c'è anche `GazeTeleportAnchorFilter`: guarda allo stesso modo, ma sfavorisce molto l'arrivo più vicino a te, così non ti lascia dove sei già. Va bene per spostarsi tra i punti di un belvedere; dal prato, invece, sulla torre più vicina non ti porterebbe quasi mai.

Chi programma può scrivere il proprio filtro: uno script con un solo metodo, che riceve il volume e restituisce il numero dell'arrivo scelto. Per esempio il più vicino, o quello davanti all'enigma ancora da risolvere.
> verifica: volume-sguardo Prato
> lavoro: 6 3

## La direzione d'arrivo
Su un'area di solito arrivi girato come sei. Seleziona il `Pavimento` e accendi **Match Directional Input**: ora, mentre punti, la direzione d'arrivo la scegli con la levetta.

Prova in Play: tieni **I** per l'arco e aggiungi **J** o **L**. La freccia del reticolo ruota, e arrivi girato come la freccia. La direzione conta rispetto a dove punta il controller: levetta a sinistra vuol dire a sinistra di dove stai puntando. Col simulatore l'ordine dei tasti conta: tieni **I**, aggiungi **J**, lascia **I** e per ultimo **J**. Arrivi girato di 90 gradi a sinistra. Se lasci prima **J**, la levetta torna dritta e arrivi senza girarti.
> evidenzia: Inspector | Match Directional Input | Pavimento
> verifica: scena-vero Pavimento | TeleportationArea | m_MatchDirectionalInput | sì
> lavoro: 4 2

# 5 · Altri modi di muoversi

## Il salto
`Locomotion/Jump` ha il **Jump Provider**: il tasto A del controller destro fa saltare. **Jump Height** è l'altezza del salto, 1,25 m; con **Variable Height Jump** il salto è più alto se tieni premuto.

Come ogni provider, lavora con un input: nel campo **Jump Input**, **Input Action Reference Performed** e **Input Action Reference Value** puntano all'azione `XRI Right Locomotion/Jump` dell'asset `XRI Default Input Actions`.

Ora smontalo e rimontalo, come si fa per aggiungere un modo di muoversi a un rig: apri il prefab `XR Origin (sabotato)`, seleziona `Locomotion/Jump`, togli il Jump Provider (i tre puntini, **Remove Component**), poi **Add Component ▸ Jump Provider**. Nel nuovo componente trascina `XRI Right Locomotion/Jump` nei due campi di **Jump Input** e salva.
> evidenzia: Inspector | Jump Input
> lavoro: 6 3

## Prova il salto
Premi Play, vai davanti al gradino da 40 cm e salta: tasto **1**, il tasto A del controller destro. Con lo Step Offset a 0,2 camminando non ci sali; saltando sì. Se il tasto non fa niente, ricontrolla i due campi di Jump Input.

Molti giochi VR non fanno saltare: il corpo resta fermo mentre gli occhi vedono un salto, e per molte persone è scomodo. Decidi tu se nel tuo progetto serve.
> verifica: sopra-oggetto Gradino da 40 cm
> attesa: 15 entrata in Play
> lavoro: 3 1

## La scala a pioli
Esci dal Play. Nella cartella della palestra c'è `Scala a pioli`: trascinala nella scena e appoggiala al bordo del soppalco verso il varco, con **Position** 2 · 0 · 15,6 e **Rotation Y** 180. La scala sale verso il suo asse Z negativo: per questo la giri verso il soppalco.

I pioli hanno un **Climb Interactable**: si afferrano col grip, e si colorano quando la mano li può prendere. Il rig ha il **Climb Provider**, in `Locomotion/Climb`, che ti fa salire quando tiri un piolo verso il basso, e sotto di lui `Climb Teleport`, con il **Climb Teleport Interactor**: quando lasci la presa ti posa in cima, sull'unico arrivo del volume della scala, perché scavalcare un bordo con le mani è scomodo.

In Play, col simulatore:
- avvicinati alla scala finché un piolo si colora;
- tieni **G**: hai afferrato il piolo;
- per salire a mano premi **]** e tieni **Q**: la mano scende e il corpo sale (con **E** succede il contrario); **Tab** per tornare;
- lascia **G**: sei in cima.

Nel visore è il gesto più naturale del mondo.
> verifica: componente-in-scena ClimbInteractable
> lavoro: 12 6

## Il grab move
Nel prefab seleziona `Locomotion/Grab Move` e attivalo, con la spunta accanto al nome. Ha tre componenti: due **Grab Move Provider**, uno per mano (**Controller Transform** = il controller, **Grab Move Input** = il grip di quella mano), e il **Two-Handed Grab Move Provider**, che li usa insieme.

Tenendo il grip trascini il mondo, come se tirassi una corda. Con due mani puoi anche ruotare (**Enable Rotation**) e cambiare scala (**Enable Scaling**). Col simulatore prova con una mano: premi **]**, così **W A S D** muovono solo il controller destro; poi tieni **G**, il grip, e insieme **A** o **D**: il controller va di lato e il mondo lo segue. Con **Tab** torni a muovere testa e mani insieme, ma lì il grab move non si vede, perché la testa si sposta con la mano. A due mani si prova meglio nel visore.

Ha senso in esperienze di osservazione o di costruzione, come un plastico, molto meno in un walking simulator.
> verifica: prefab-attivo XR Origin (sabotato) | Locomotion/Grab Move
> lavoro: 3 2

# 6 · Comfort

## La vignetta
Nel prefab trascina `TunnelingVignette`, dalla cartella `Assets/Samples/XR Interaction Toolkit/3.6.1/Starter Assets/TunnelingVignette`, come figlio di `Camera Offset/Main Camera`.

Nel suo **Tunneling Vignette Controller**, nella lista **Locomotion Vignette Providers**, aggiungi due elementi e trascina `Locomotion/Move` e `Locomotion/Turn` nel campo **Locomotion Provider** di ciascuno. Durante il movimento i bordi della visuale si scuriscono: meno movimento nella visione periferica, meno nausea.

Regola **Aperture Size**, quanto resta visibile, e **Feathering Effect**, quanto è sfumato il bordo, poi prova in Play con e senza vignetta.
> verifica: prefab-numero XR Origin (sabotato) | Camera Offset/Main Camera/TunnelingVignette | TunnelingVignetteController | m_LocomotionVignetteProviders.Array.size | 2 | 9
> lavoro: 10 5

# Da soli · Verifica

## Il percorso
Ora tocca a te, senza istruzioni passo passo. Premi il pulsante: si apre la scena `Verifica`, un percorso a ostacoli dal punto A al punto B.

Le regole:
- l'**ambiente non si sposta**: niente oggetti spostati, ruotati o scalati;
- puoi **aggiungere** oggetti e componenti: aree, anchor e volumi; dalla cartella della palestra, la scala a pioli, l'anchor che si colora quando lo punti e il Multi-Anchor Volume;
- il rig della verifica, `XR Origin (verifica)`, ha **dei pezzi mancanti o spenti**: aggiungili, collegali e accendili, come hai fatto nella lezione;
- anche la pedana della porta 1, che ha la forma del Teleport Anchor della lezione, il piedistallo nella fossa dell'atrio e il belvedere, fatto come il Multi-Anchor Volume della palestra, non hanno componenti di teleport.

Qui sotto c'è cosa devi ottenere, non come. I compiti si spuntano da soli mentre lavori, e in alto vedi il punteggio. Le prove in Play si fanno col **Simulatore VR** acceso (RUFA ▸ Play mode ▸ Simulatore VR), oppure col visore e Quest Link (RUFA ▸ Play mode ▸ VR con Quest Link): col rig desktop nessun compito in Play può riuscire. Quello che ottieni in Play resta spuntato. È un'autovalutazione: serve a te.
> azione: Apri la verifica | apri-scena | Lezione 02 - Locomozione | Verifica

## La vignetta
Prima di partire, il comfort.

**Risultato atteso:** nel rig della verifica i bordi della visuale si scuriscono quando cammini e quando giri.
> verifica: prefab-numero XR Origin (verifica) | Camera Offset/Main Camera/TunnelingVignette | TunnelingVignetteController | m_LocomotionVignetteProviders.Array.size | 2 | 9
> lavoro: 3 2

## La torretta
Parti chiuso in una gabbia di sbarre in cima a una torretta: vedi fuori, ma non passi. L'unica uscita è la botola nel pavimento: sotto c'è la piattaforma, e da lì una porta dà sulla `Partenza`.

**Risultato atteso:** in Play cadi dalla botola sulla piattaforma, e la porta della torretta si apre.
> verifica: riuscito Porta della torretta
> lavoro: 3 1

## La gravità nel rig
**Risultato atteso:** nel rig della verifica la gravità è accesa.
> verifica: prefab-vero XR Origin (verifica) | Locomotion/Gravity | GravityProvider | m_UseGravity | sì

## Sulla sponda
Oltre il baratro c'è la sponda, ma non accetta il teleport. Se cadi nel baratro hai due strade: col teleport del controller destro punti la `Partenza`, davanti alla torretta, e torni su; oppure esci dal Play e rientri, e ricominci dall'inizio.

Attento ai fianchi: un'area accetta il teleport su tutto il suo collider, e dal fondo del baratro basterebbe puntare la parete della sponda per ritrovarsi sopra.

**Risultato atteso:** in Play arrivi sulla sponda, e la sua area scarta i punti inclinati più di 30°, così ci si arriva solo sulla superficie.
> verifica: sopra-area-filtrata Sponda
> lavoro: 4 2

## La porta 1
La porta 1 si apre solo a chi arriva col teleport sulla pedana davanti, già girato verso la porta. Chi ci arriva camminando e poi si gira resta fuori.

**Risultato atteso:** in Play la porta si apre.
> verifica: riuscito Porta 1
> lavoro: 4 2

## Un arrivo progettato per la porta 1
**Risultato atteso:** sulla pedana 1 c'è un punto d'arrivo che fa arrivare già rivolti alla porta.
> verifica: anchor-verso Porta 1

## La porta 2
Al centro dell'atrio c'è una fossa, e nel mezzo un piedistallo con una freccia che indica la porta 2, nel muro di sinistra. La porta si apre solo a chi arriva col teleport sul piedistallo girato come la freccia. Parti dalla freccia sul bordo della fossa: da lì arriveresti girato verso il fondo, e la direzione devi sceglierla tu. Col simulatore: tieni **I**, aggiungi **J**, lascia **I** e per ultimo **J**. Se cadi nella fossa, risali col teleport.

**Risultato atteso:** in Play la porta 2 si apre.
> verifica: riuscito Porta 2
> lavoro: 4 2

## La direzione scelta da chi gioca
**Risultato atteso:** sul piedistallo il teleport fa scegliere la direzione d'arrivo con la levetta.
> verifica: scena-vero Piedistallo | TeleportationArea | m_MatchDirectionalInput | sì

## Il belvedere
Il belvedere è alto 1,5 m: a piedi non ci sali. Ed è fragile: regge solo sui suoi tre dischi, e chi atterra altrove viene rimandato indietro. È fatto come il Multi-Anchor Volume della palestra, con la pedana e i tre arrivi, ma senza il componente.

**Risultato atteso:** in Play arrivi su uno dei tre dischi.
> verifica: riuscito Belvedere
> lavoro: 6 3

## Un volume per il belvedere
**Risultato atteso:** sul belvedere c'è un Multi-Anchor Volume con i suoi tre arrivi in lista.
> verifica: componente-su TeleportationMultiAnchorVolume | Belvedere/Pedana

## Il gradino da 60 cm
Dal belvedere si esce su una passerella, e alla fine c'è un gradino da 60 cm.

**Risultato atteso:** in Play sei sopra il gradino.
> verifica: sopra-oggetto Gradino da 60 cm
> lavoro: 4 2

## Il salto nel rig
**Risultato atteso:** il rig della verifica ha il suo provider del salto, collegato all'input.
> verifica: prefab-componente XR Origin (verifica) | Locomotion | JumpProvider

## Il balcone
In fondo alla terrazza c'è una parete alta 2,5 m, con un balcone in cima.

**Risultato atteso:** in Play arrivi sul balcone.
> verifica: sopra-oggetto Balcone
> lavoro: 8 4

## La scala nel rig
**Risultato atteso:** il rig della verifica sa arrampicarsi, e quando lasci la scala ti posa sul balcone.
> verifica: prefab-componente XR Origin (verifica) | Locomotion | ClimbProvider

## La stanza del plastico
Questa stanza si attraversa trascinando il mondo con le mani.

**Risultato atteso:** in Play ti muovi nella stanza col grab move.
> verifica: riuscito Stanza del plastico
> lavoro: 5 2

## L'arrivo
**Risultato atteso:** in Play arrivi al punto B, oltre la stanza del plastico.
> verifica: sopra-oggetto Arrivo
> lavoro: 3 1

## L'ambiente intatto
**Risultato atteso:** nessun oggetto dell'ambiente spostato, ruotato o scalato.
> verifica: ambiente-intatto Ambiente

## Extra: gli anelli
Oltre l'arrivo, sopra un prato, tre anelli d'oro galleggiano nell'aria. Passaci dentro, uno dopo l'altro: a piedi non ci arrivi, bisogna volare. È un punto in più, fuori dal punteggio.

**Risultato atteso:** in Play passi dentro tutti e tre gli anelli.
> verifica: riuscito Anelli

# Da soli · Il tuo progetto

## La tua scena
Il progetto d'esame lo costruisci lezione dopo lezione, in una scena tutta tua. Premi il pulsante: Unity crea in `Assets/Progetto` la scena `Progetto`, con lo stretto necessario (il tuo rig, un pavimento e la luce), e il tuo rig, `XR Origin (progetto)`. La scena diventa la prima della build: **RUFA ▸ Build ▸ Quest (APK)** porta sul visore il tuo progetto.

Se la scena c'è già, il pulsante la riapre e basta: non tocca niente. `git aggiorna` non tocca la cartella `Assets/Progetto`.

A fine lezione chiudi Unity e salva il tuo lavoro: `git salva "Lezione 02"`.
> azione: Crea la scena del progetto | crea-progetto
