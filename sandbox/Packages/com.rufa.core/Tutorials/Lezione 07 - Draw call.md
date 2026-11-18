Lezione 7 · Le draw call nel mercato coperto. Le note prima del primo capitolo non si vedono nella finestra. Stima per lo studente lento (tools/stima-tempi.py): capitoli insieme 0-4 circa 105', il laboratorio (capitolo 5) circa 76', verifica circa 26'. La giornata è negli appunti del docente. Parla Gianluca in prima persona e dà del tu allo studente. Nessun numero misurato su un PC: solo quelli di Meta e di Unity, e quelli costruiti nella scena (i budget dei cartelli, i materiali dei banchi). Si misura in Play, con le camere fisse sui Display 2-6 della Game view: lo static batching Unity lo fa quando parte il Play. Si cura fuori dal Play.

# 0 · Prima di iniziare

## Cosa farai in questa lezione
Oggi lavori per un altro cliente. Il mercato coperto della città vuole una visita in realtà virtuale, sul Quest 3S. I banchi sono pieni e belli: frutta, spezie, dolci, un bar, cassette regalo.
Il problema non sono i triangoli, quelli li hai curati nella lezione 6. Il problema è il numero di cose da disegnare. Ogni frutto è un oggetto, ogni oggetto ha il suo materiale, e il visore li deve disegnare uno per uno.
Il tuo lavoro è far stare il mercato nel budget del cliente senza che cambi aspetto. Il banco della frutta lo sistemiamo insieme. Dopo la pausa, nel laboratorio, fai in Blender un **atlas**: le texture di più oggetti messe in un'immagine sola. Poi tre banchi li sistemi da solo, e un compagno farà il curatore, come nella lezione 6.
Anche questo ti capiterà davvero. Meta dà un budget di draw call per ogni tipo di gioco. VRChat conta i materiali di ogni avatar per Quest, e oltre un certo numero l'avatar non si vede. Uno studio ti dirà quanti materiali può avere il tuo modello prima ancora che tu cominci.

## I modelli nuovi
Questa pagina controlla che nel tuo progetto ci siano le merci del mercato, in `Assets/Demo/Models/Mercato`.
Se qui sotto c'è scritto **Fatto**, vai avanti. Se resta **Da fare**, chiudi Unity, lancia `git aggiorna` un'altra volta nella cartella della tua copia e riapri il progetto. Il primo `git aggiorna` di oggi porta il comando nuovo, il secondo porta i file. Unity Hub deve essere aperto: il progetto deve riscaricare i suoi package.
> verifica: asset-esiste Assets/Demo/Models/Mercato/food_apple_01.fbx

## Il rig del PC
Oggi misuri in Play. Il visore resta spento: usiamo il rig del PC.
1. Dal menu in alto apri **RUFA ▸ Play mode**. Se accanto a **Simulatore VR** c'è la spunta, clicca la voce: la spunta sparisce.
In Play clicca nella Game view per guardare col mouse e cammina con **W A S D**. **Esc** ti ridà il mouse.
> verifica: simulatore-spento

## L'editor sul PC
I budget di oggi sono pensati per l'editor impostato per il PC. Se l'ultima volta hai fatto una build per il visore, l'editor è rimasto su Android: disegna come il visore, i numeri vengono diversi e l'esperimento del capitolo 2 non funziona.
Se qui sotto resta **Da fare**:
1. Dal menu in alto apri **File ▸ Build Profiles**.
2. A sinistra, fra le piattaforme, scegli **Windows** e premi **Switch Platform**. Unity ci mette qualche minuto.
> verifica: piattaforma-pc

## Apri il mercato
Premi il pulsante qui sotto. Unity copia il mercato nel tuo progetto, in `Assets/Samples/RUFA Core/Lezione 07 - Draw call`, e apre la scena `Mercato`.
La copia è tua: puoi cambiarla quanto vuoi. Se la rovini, cancella quella cartella e premi di nuovo il pulsante.
> azione: Apri il mercato | apri-scena | Lezione 07 - Draw call | Mercato
> verifica: scena-aperta Mercato

## Le cinque camere
Lungo la navata ci sono cinque botteghe: la frutta, le spezie, la pasticceria, il bar e le cassette miste. Ognuna ha una camera fissa sull'apertura, che guarda dentro e vede solo la sua bottega.
1. Clicca la linguetta **Game** per mettere la Game view davanti.
2. In alto a sinistra nella Game view c'è un menu con scritto **Display 1**. Aprilo e scegli **Display 2**: compare la frutta, vista dalla sua camera.
3. Accanto c'è il menu delle proporzioni, quello con scritto **Free Aspect**. Scegli **16:9 Aspect**. Così l'inquadratura è la stessa su tutti i PC.
4. In alto a destra nella Game view premi **Stats**. Compare un riquadro coi numeri del frame.
Le spezie sono sul **Display 3**, la pasticceria sul **Display 4**, il bar sul **Display 5**, le cassette sul **Display 6**. Il **Display 1** è il tuo rig, in Play.
> lavoro: 2 1

## La tua insegna
Nella bottega della frutta, in alto sulla parete di fondo, c'è un'insegna bianca. È per te. Se hai un tuo disegno, una tua grafica o una tua foto, mettila lì.
1. Trascina il file della tua immagine, PNG o JPG, dall'Esplora file alla cartella `Assets/Progetto` nella finestra **Project**.
2. Nella Hierarchy apri `Banco della frutta` e seleziona `Tela`.
3. In fondo all'Inspector c'è il materiale `Insegna`. Aprilo con la freccia. Sotto **Surface Inputs**, accanto a **Base Map**, c'è un quadratino: trascinaci sopra la tua immagine.
Se oggi non hai un'immagine con te, salta questa pagina. L'insegna resta bianca.
> lavoro: 3 2

## Il file delle misure
Oggi scrivi i numeri che leggi in un file, `Consegne/L07/misure.md`. Premi il pulsante: il file si crea e si apre nel blocco note. Tienilo aperto, ci torni spesso.
> azione: Crea il file delle misure | crea-misure | Consegne/L07/misure.md
> verifica: file-esiste Consegne/L07/misure.md

# 1 · Il budget

## Una draw call è un ordine
Per disegnare un oggetto la CPU, il processore, deve dire alla GPU, la scheda grafica: «disegna questa mesh con questo materiale». Quest'ordine si chiama **draw call**. Ogni oggetto costa almeno una draw call, e un oggetto con tre materiali ne costa tre.
Prima dell'ordine la CPU prepara la GPU: lo shader giusto, le texture, le impostazioni del materiale. Questo cambio si chiama **SetPass**, e costa più della draw call. Se due oggetti di fila usano lo stesso shader, il secondo non ha bisogno di un SetPass.
Nel visore la CPU è piccola, e ha poco tempo: a 90 frame al secondo ha 11 millisecondi per frame. Tanti oggetti vuol dire tanti ordini, e il tempo finisce prima del frame.

## Il budget di Meta
Meta dice quante draw call può reggere un Quest 3 o un 3S in un frame, secondo il tipo di app:
- da 200 a 300 se l'app simula tanto, come uno sparatutto in multigiocatore o un'app sociale piena di persone;
- da 400 a 600 per la maggior parte delle app;
- da 700 a 1000 per le app leggere: Meta cita le escape room, i puzzle e i giochi cooperativi.
È il totale del frame, per tutta la scena. Il nostro cliente l'ha diviso fra i banchi e l'ha scritto su una lavagna dentro ogni bottega.
Se il tuo progetto è una escape room, il tuo budget sta in quella terza riga. Se è un walking simulator con poca simulazione, anche.

## Cosa conta Stats
Nel riquadro **Stats** oggi guardi tre numeri:
- **Batches**: le draw call del frame, dopo che Unity ne ha messe insieme quante poteva;
- **Saved by batching**: quante draw call Unity ha risparmiato mettendole insieme;
- **SetPass calls**: i cambi di shader e di stato.
Come per i triangoli, Stats conta ogni passata. Quando l'editor è impostato per il PC, il nostro progetto disegna gli oggetti opachi una volta in più, per l'occlusione ambientale (SSAO). Per questo sul PC vedi circa il doppio delle draw call. I budget delle lavagne sono pensati così; sul visore hai più margine.

## Si misura in Play
Fuori dal Play Unity disegna ogni oggetto per conto suo. Una delle cure di oggi, lo **static batching**, Unity la fa quando parte il Play, o quando costruisce l'app per il visore. Per questo oggi misuri sempre in Play.
La regola è questa: misuri in Play, curi fuori dal Play. Quello che cambi in Play sparisce quando esci, come hai visto nella lezione 5.
1. Premi **Play**.
2. Nella Game view scegli **Display 2**: la frutta. Se il menu non risponde, premi **Esc** per riavere il mouse.
3. Leggi **Batches**, **Saved by batching** e **SetPass calls**, e scrivili nel file delle misure, alla riga della frutta.
4. Confronta **Batches** col budget sulla lavagna. È molto più alto.
5. Premi di nuovo **Play** per uscire.
> verifica: fuori-play
> lavoro: 3 2

## La foto del prima
La foto è un'immagine di quello che vede la camera della frutta. Il pulsante la salva in `Consegne/L07/frutta-prima.jpg`, accanto al file delle misure.
> azione: Foto della frutta, prima | foto-camera | Camera Frutta | Consegne/L07/frutta-prima.jpg
> verifica: file-esiste Consegne/L07/frutta-prima.jpg

## Il Frame Debugger
Stats ti dice quante draw call ci sono. Il **Frame Debugger** ti fa vedere quali sono, una per una.
1. Premi **Play** e scegli **Display 2**.
2. Dal menu in alto apri **Window ▸ Analysis ▸ Frame Debugger** e premi **Enable**. Il gioco si ferma su un frame.
3. A sinistra c'è l'elenco di tutto quello che Unity ha disegnato. Apri le righe col triangolino finché trovi le **SRP Batch**: sono gruppi di draw call che Unity ha preparato insieme.
4. Clicca un gruppo. A destra, **Batch cause** dice perché quel gruppo si è interrotto e non ha preso anche l'oggetto dopo. Al banco della frutta leggerai spesso che lo shader o le sue opzioni cambiano.
5. Trascina il cursore in alto nel Frame Debugger: la Game view disegna il frame fino a quel punto, un ordine alla volta, e vedi i frutti comparire uno per uno.
6. Premi **Disable** ed esci dal Play.
Il Frame Debugger è il primo strumento da aprire quando le draw call sono troppe: prima capisci, poi curi.
> lavoro: 5 3

# 2 · Uno shader, un materiale

## L'SRP Batcher
URP ha un aiutante, l'**SRP Batcher**. Non riduce le draw call: riduce il lavoro fra una e l'altra. Gli oggetti che usano lo stesso shader con le stesse opzioni li prepara tutti insieme, con un solo SetPass, anche se ognuno ha il suo materiale.
Unity lo dice così: usa meno varianti possibile dello shader, e tutti i materiali che vuoi con lo stesso shader. Meta dice la stessa cosa: pochi shader, unificati, che non creano varianti.
Nel nostro progetto l'SRP Batcher è acceso. Ora lo spegni per un momento, per vedere quanto lavora.

## L'esperimento · spegnilo
1. Nella finestra **Project** apri `Assets/Settings` e seleziona `PC_RPAsset`: sono le impostazioni di URP che l'editor usa per il PC.
2. Nell'Inspector, nel riquadro **Rendering**, clicca i tre puntini a destra del titolo e scegli **Show All Advanced Properties**. Compare la casella **SRP Batcher**.
3. Togli la spunta a **SRP Batcher**.
4. Premi **Play**, scegli **Display 2** e leggi **SetPass calls**: è esploso, centinaia, più dei frutti. Scrivilo nel file delle misure, alla riga dell'esperimento. Esci dal Play.
> verifica: srp-batcher-spento
> lavoro: 3 2

## L'esperimento · riaccendilo
Nell'Inspector di `PC_RPAsset` rimetti la spunta a **SRP Batcher**.
Questa pagina controlla che sia di nuovo acceso. Se lo lasci spento, peggiori anche il tuo progetto.
> verifica: srp-batcher-acceso

## Gli shader di URP
URP ha più shader per le superfici illuminate. Dal più pesante al più leggero:
- **Complex Lit**: Lit con in più lo strato trasparente delle vernici lucide. Unity consiglia di evitarlo quando contano le prestazioni.
- **Lit**: lo shader realistico, il più completo che usi di solito.
- **Simple Lit**: una luce più semplice e un riflesso meno fisico. Unity lo consiglia quando le prestazioni contano più del fotorealismo, ed è il caso del visore.
- **Baked Lit**: solo la luce cotta nelle lightmap, per gli oggetti fermi. Lo vediamo nella lezione 8.
- **Unlit**: nessuna luce, solo il colore. Il più leggero di tutti.
Al banco della frutta ogni tipo di frutto è arrivato con uno shader diverso: chi su Lit, chi su Complex Lit, chi su Simple Lit.

## Le varianti
Uno shader è una famiglia. Ogni opzione che accendi nel materiale crea una **variante**, cioè una versione diversa dello stesso shader: una normal map, i riflessi speculari, l'**Enable GPU Instancing**. Due materiali con lo stesso shader ma una normal map solo nel primo sono due varianti, e l'SRP Batcher li prepara separati.
Per questo la cura è: un solo shader, con le stesse opzioni per tutto il banco.

## Le copie
Al banco della frutta ogni mela ha la sua copia del materiale: `Mela`, `Mela.001`, `Mela.002`… Sono i nomi che dà Blender quando importi lo stesso modello più volte: il fruttivendolo ha importato ogni cassetta per conto suo.
Con l'SRP Batcher acceso le copie non costano SetPass. Però bloccano lo static batching, che usiamo più avanti e che mette insieme solo gli oggetti con lo stesso materiale. Quindi le copie si fondono: un materiale per ogni tipo di frutto.
1. Nella Hierarchy, nella casella di ricerca in alto, scrivi `Mela`. Restano solo le mele.
2. Clicca la prima, poi **Shift**+clic sull'ultima: sono tutte selezionate.
3. Nella finestra **Project** apri `Assets/Samples/RUFA Core/Lezione 07 - Draw call/Materiali/Frutta`. Trascina il materiale `Mela` sull'Inspector, sopra il campo del materiale, nel componente **Mesh Renderer**, alla voce **Materials**.
4. Fai lo stesso per `Pere`, `Limone`, `Lime`, `Kiwi`, `Avocado`, `Melograno`, `Litchi`, `Cesto` e `Fruttiera`, ognuno col suo materiale senza numero.
5. Svuota la ricerca con la crocetta e salva con **Ctrl+S**.
Se la pagina resta su **Da fare**, è rimasto un frutto con una copia: cercalo coi nomi. Oppure un frutto è stato spostato o spento: annulla con **Ctrl+Z**.
> verifica: materiali-al-massimo Frutta | 10
> lavoro: 7 4

## Uno shader per tutta la frutta
1. Nella cartella `Materiali/Frutta` seleziona tutti i materiali: clicca il primo e premi **Ctrl+A**.
2. In cima all'Inspector, nel menu **Shader**, scegli **Universal Render Pipeline ▸ Simple Lit**.
3. In fondo, sotto **Advanced Options**, metti la spunta a **Specular Highlights** e togli quella a **Enable GPU Instancing**. Se una casella mostra un trattino, i materiali sono diversi: cliccala finché è come la vuoi. Dopo il cambio di shader **Specular Highlights** mostra quasi sempre il trattino: Lit e Simple Lit leggono quell'opzione al contrario.
4. Guarda la frutta nella Game view: è uguale a prima? Se un frutto ti sembra troppo lucido o troppo opaco, selezionane il materiale e regola **Smoothness**.
Se la pagina resta su **Da fare**, un materiale è rimasto con un'opzione diversa: selezionali tutti di nuovo e cerca i trattini.
> verifica: una-variante Frutta | Universal Render Pipeline/Simple Lit
> lavoro: 5 3

## Rileggi i numeri
Premi **Play**, scegli **Display 2** e rileggi i tre numeri. Scrivili nel file.
**SetPass calls** è sceso. **Batches** invece è quasi uguale: l'SRP Batcher prepara meglio le draw call, ma non ne toglie nessuna. Per toglierle serve il capitolo dopo.
Esci dal Play.
> verifica: fuori-play
> lavoro: 2 1

# 3 · Fermi e uniti

## Lo static batching
La frutta del banco non si muove mai. Unity può allora unire le sue mesh in una mesh grande, una volta sola, e disegnare tanti frutti con poche draw call. Si chiama **static batching**.
Servono due cose:
- il flag **Batching Static** sugli oggetti, nella casella **Static** dell'Inspector;
- l'opzione **Static Batching** nelle impostazioni del Player, che nel nostro progetto è già accesa.
Unity unisce solo gli oggetti con lo stesso materiale: per questo prima hai fuso le copie.
Ha dei costi. Unity tiene in memoria una copia delle mesh unite, quindi la memoria cresce. Un gruppo arriva al massimo a 64.000 vertici, poi ne comincia un altro. E un oggetto statico non si può più spostare in gioco.

## La frutta ferma
1. Nella Hierarchy apri `Banco della frutta` e seleziona `Frutta`.
2. In alto a destra nell'Inspector c'è la casella **Static** con una freccia. Apri la freccia e scegli **Batching Static**.
3. Unity chiede se vale anche per i figli: rispondi **Yes, change children**.
4. Salva con **Ctrl+S**.
> verifica: statici Frutta
> lavoro: 2 1

## Rileggi in Play
Premi **Play**, scegli **Display 2** e rileggi i tre numeri. Ora **Batches** è crollato e **Saved by batching** è salito. Scrivili nel file ed esci dal Play.
Nel Frame Debugger, se vuoi, clicca una draw call della frutta: nei dettagli, alla voce della mesh, compare **Combined Mesh**, la mesh unita dello static batching, che disegna tanti frutti insieme.
> verifica: fuori-play
> lavoro: 3 2

## L'atlas e l'unione a mano
Lo static batching mette insieme oggetti con lo stesso materiale. Ma due materiali uguali in tutto tranne la texture restano due. La soluzione è l'**atlas**: le texture dei due materiali si mettono in un'immagine sola, più grande, e i due materiali diventano uno. Unity lo consiglia proprio così, e Meta pure: condividi i materiali, e usa un atlas quando puoi. Lo fai nel laboratorio, dopo la pausa.
Si può anche andare oltre e unire a mano le mesh in un oggetto solo, nel programma 3D. Fa una draw call sola, ma c'è un prezzo: l'oggetto unito si disegna tutto o niente. Se ne vedi un pezzo, Unity disegna anche il resto, quello alle tue spalle compreso. Unisci quello che sta sempre insieme, come le gambe e il piano di un tavolo; non un'intera stanza.

## L'Instancing, un falso amico
Nell'Inspector di ogni materiale c'è **Enable GPU Instancing**. Il nome promette di disegnare tante copie dello stesso oggetto con una draw call. Con URP e l'SRP Batcher acceso però non serve: Unity usa l'SRP Batcher e ignora l'Instancing. In più la casella aggiunge varianti allo shader, e Unity consiglia di lasciarla spenta.
L'Instancing serve in casi particolari, con l'SRP Batcher spento o con shader scritti apposta. Al bar lo proverai tu, e vedrai che Stats non cambia.

## Il GPU Resident Drawer e il dynamic batching
In Unity 6 ci sono altri due modi di mettere insieme le draw call. Nel nostro progetto sono spenti tutti e due, ed è giusto così.
Il **GPU Resident Drawer** lascia alla GPU il lavoro di preparare le draw call. Funziona solo con **Forward+** e con le GPU che fanno i calcoli generici. Unity sconsiglia di usarlo sui visori autonomi come il Quest.
Il **dynamic batching** unisce al volo le mesh piccole che si muovono. Unity non lo consiglia più: il lavoro che chiede alla CPU può costare più della draw call che risparmia.

# 4 · Vetri e trasparenze

## Il vetro costa
Un oggetto opaco copre quello che ha dietro: la GPU colora ogni pixel una volta sola. Un oggetto trasparente invece si disegna sopra quello che c'è dietro, e mescola i colori. Ogni strato di vetro ricolora gli stessi pixel. Si chiama **overdraw**, e sul visore i pixel sono tanti.
In più gli oggetti trasparenti si disegnano dal più lontano al più vicino, e Unity li può mettere insieme molto meno degli opachi.
Meta lo dice chiaro: la trasparenza non è gratis, tienila al minimo. E c'è un'altra regola: evita l'**alpha test**, il ritaglio dei pixel che si usa per foglie e reti. Sul visore costa più del vetro, e Meta consiglia di sostituirlo con la trasparenza dove si può. Unity dice lo stesso a chi scrive shader: evita le istruzioni che scartano i pixel.

## Vedere l'overdraw
1. Premi **Play** e scegli **Display 2**.
2. Dal menu in alto apri **Window ▸ Analysis ▸ Rendering Debugger**. Trascina la sua linguetta sul bordo destro della Game view e lascia andare, così le due finestre stanno affiancate.
3. Nella colonna di sinistra clicca **Rendering**. Nella riga **Overdraw Mode** scegli **Transparent**.
4. Guarda la Game view: è diventata scura, e dove ci sono strati trasparenti si accende. Più è chiaro, più strati ci sono. La vetrinetta sul bancone brilla più del dovuto.
5. Rimetti **Overdraw Mode** su **None** ed esci dal Play.
> lavoro: 3 2

## La vetrinetta
La vetrinetta dei frutti delicati ha ogni lastra doppia: chi l'ha modellata ha fatto due lastre a pochi millimetri per dare spessore al vetro. Lo strato interno costa una draw call e uno strato di overdraw, e non si vede.
1. Nella Hierarchy apri `Banco della frutta` e poi `Vetri della frutta`.
2. Seleziona le cinque lastre che finiscono con **interna**: clicca la prima e **Ctrl**+clic sulle altre.
3. Premi **Canc** per cancellarle.
4. Ora ogni lato ha un vetro solo invece di due, e la vetrinetta sembra più limpida. Per avere lo stesso vetro di prima, seleziona una lastra, apri il materiale `Vetro`, clicca il colore accanto a **Base Map** e porta **A**, l'opacità, da 12 a 23: un vetro solo, spesso il doppio. Il materiale è lo stesso per tutti i vetri del mercato.
5. Salva con **Ctrl+S**.
Se la pagina resta su **Da fare**, hai cancellato troppo o troppo poco: annulla con **Ctrl+Z** e riprova. I frutti non vanno toccati.
> verifica: trasparenti-fra Vetri della frutta | 4 | 5 | Frutta
> lavoro: 3 2

## Il dopo della frutta
1. Premi **Play**, scegli **Display 2** e rileggi i tre numeri. Ora **Batches** è sotto il budget della lavagna.
2. Scrivili nel file delle misure ed esci dal Play.
3. Fai la foto del dopo.
4. Salva la scena con **Ctrl+S**.
> azione: Foto della frutta, dopo | foto-camera | Camera Frutta | Consegne/L07/frutta-dopo.jpg
> verifica: file-esiste Consegne/L07/frutta-dopo.jpg

## Il curatore
Il curatore è il tuo compagno di banco, come nella lezione 6, e tu lo fai per lui.
1. Apri la cartella `Consegne/L07` nell'Esplora file. Ci sono le due foto della frutta.
2. Decidi tu in che ordine mostrarle. Aprile una alla volta a tutto schermo, così il compagno non vede il nome del file.
3. Il compagno dice quale è quella curata, e perché: cosa vede di diverso.
Se ti indica una differenza che c'è davvero, per esempio un frutto meno lucido, torna al banco e correggila. Se tira a indovinare, hai fatto un buon lavoro. Poi scambiatevi i ruoli.
> lavoro: 4 3

# 5 · Il laboratorio

## Le cassette miste
In fondo a destra nella navata c'è il banco delle cassette regalo: dodici cassette, cinque frutti in ognuna. È un caso che troverai spesso: un modello fatto di pezzi, ognuno col suo materiale. Ogni pezzo costa una draw call, quindi ogni cassetta ne costa cinque, più il vassoio.
La cura si fa dove nasce il problema, nel programma 3D. In Blender fai un **atlas**: le cinque texture diventano un'immagine sola, e i cinque frutti un oggetto solo con un materiale solo. È il lavoro che fa chi prepara i modelli per un gioco.
Le cassette sono copie di un solo prefab, `Cassetta mista`, che usa il modello `cassetta_mista.fbx`. Quando riesporti il modello, si aggiornano tutte.
> lavoro: 2 1

## Le cassette · il prima
1. Premi **Play** e scegli **Display 6**: le cassette.
2. Leggi **Batches** e **SetPass calls** e scrivili nel file delle misure, alla riga delle cassette. Esci dal Play.
3. Fai la foto del prima.
> azione: Foto delle cassette, prima | foto-camera | Camera Cassette | Consegne/L07/cassette-prima.jpg
> verifica: file-esiste Consegne/L07/cassette-prima.jpg

## In Blender · importa e unisci
1. Apri Blender. Col mouse sulla vista 3D premi **A**, poi **X**, e scegli **Delete**: via il cubo, la camera e la luce. Se restano, finiscono selezionati coi frutti, e il bake si ferma.
2. Dal menu **File ▸ Import ▸ FBX (.fbx)** apri `cassetta_mista.fbx`. Sta nella cartella della tua copia, in `sandbox/Assets/Demo/Models/Mercato`. Blender trova da solo le texture, nella cartella `cassetta_mista.fbm` accanto al modello.
3. I cinque frutti arrivano selezionati, ma nessuno è attivo, e senza un oggetto attivo l'unione non funziona. Nell'Outliner, in alto a destra, si chiamano `Frutto 1`, `Frutto 2` e così via. Clicca un frutto, poi premi **A**: ora sono tutti selezionati, e quello che hai cliccato è l'attivo.
4. Premi **Ctrl+J**: i cinque frutti diventano un oggetto solo, col nome del frutto che hai cliccato. A destra, nelle **Properties**, apri la scheda **Material**, quella con la sfera: ci sono ancora cinque materiali.
Un oggetto con cinque materiali costa ancora cinque draw call: Unity disegna ogni materiale per conto suo. Il prossimo passo è dare a tutti un materiale solo.
> lavoro: 5 3

## In Blender · una UV map nuova
Ogni frutto usa tutta la sua texture: le sue isole UV, i pezzi della sua superficie stesi sul piano, occupano tutto il quadrato. Messi insieme, i cinque frutti hanno le isole una sopra l'altra. Serve una seconda UV map, dove le isole stanno una accanto all'altra.
1. Nelle **Properties** apri la scheda **Object Data**, quella col triangolo verde, e apri il riquadro **UV Maps**. C'è `UVMap`.
2. Premi **+**: nasce `UVMap.001`, evidenziata. L'icona della fotocamera resta su `UVMap`: è la mappa che i materiali leggono ancora. Fai doppio clic su `UVMap.001` e chiamala `Atlas`.
3. In alto passa alla scheda **UV Editing**: Blender entra da solo in Edit Mode. Col mouse sulla vista 3D, a destra, premi **A**: tutto selezionato.
4. Col mouse sull'editor UV, a sinistra, premi **A**: anche lì tutto selezionato. Senza questo passo il comando dopo non fa niente.
5. Nell'editor UV apri **UV ▸ Pack Islands**. Nella finestrella metti **Margin** a `0.005` e premi **OK**. Le isole dei cinque frutti si sistemano nel quadrato, senza sovrapporsi.
6. Col mouse sulla vista 3D premi **Tab** per tornare in Object Mode.
Il margine lascia qualche pixel fra un'isola e l'altra. Senza, da lontano i colori di due frutti si mescolano sui bordi.
> lavoro: 6 4

## In Blender · l'immagine dell'atlas
Ora prepari l'immagine vuota in cui Blender disegnerà le cinque texture.
1. Nell'editor a sinistra apri **Image ▸ New**. Chiamala `cassetta_mista_atlas`, lascia **1024** per **1024**, togli la spunta ad **Alpha** e premi **OK**.
2. In alto passa alla scheda **Shading**. A destra, nella scheda **Material**, clicca il primo dei cinque materiali.
3. Col mouse sull'editor dei nodi, in basso, premi **Shift+A**, scegli **Texture ▸ Image Texture** e posa il nodo in un punto vuoto. Nel nodo apri il menu delle immagini e scegli `cassetta_mista_atlas`. Non collegarlo a niente.
4. Fai lo stesso per gli altri quattro materiali.
Il nodo che hai posato per ultimo è quello attivo, col bordo chiaro: Blender cuoce nell'immagine del nodo attivo di ogni materiale.
> lavoro: 8 5

## In Blender · il bake del colore
Il **bake** è la cottura: Blender guarda ogni frutto con la sua texture e la sua UV map di sempre, e disegna quello che vede nell'immagine nuova, seguendo la UV map `Atlas`.
1. Nelle **Properties** apri la scheda **Render**, quella con la macchina fotografica sul retro. In **Render Engine** scegli **Cycles**.
2. Sotto **Sampling ▸ Render** metti **Max Samples** a `16`. Per copiare un colore bastano pochi campioni, e il bake va molto più veloce.
3. Più in basso apri il riquadro **Bake**. In **Bake Type** scegli **Diffuse**. Sotto **Contributions** togli la spunta a **Direct** e **Indirect** e lascia **Color**: vuoi il colore della texture, senza luci né ombre. **Margin** resta a 16 px.
4. Nella vista 3D il frutto unito deve essere selezionato. Premi **Bake**. In pochi secondi l'immagine si riempie dei cinque frutti.
5. Torna alla scheda **UV Editing**: a sinistra c'è l'atlas, ora pieno dei frutti. Scegli **Image ▸ Save As**, il formato **PNG**, la cartella `sandbox/Assets/Demo/Models/Mercato` e il nome `cassetta_mista_atlas.png`.
Blender non salva da solo le immagini cotte. Se chiudi senza salvarle, le perdi.
> lavoro: 6 4

## In Blender · il bake della normal map
Anche il rilievo dei frutti, che sta nelle normal map, deve finire in un'immagine sola.
1. Nella scheda **UV Editing**, **Image ▸ New**: chiamala `cassetta_mista_atlas_normal`, 1024 per 1024, senza **Alpha**, **OK**. Poi torna alla scheda **Shading**.
2. In ognuno dei cinque materiali, nel nodo **Image Texture** che hai posato, scegli `cassetta_mista_atlas_normal` al posto dell'altra immagine.
3. In uno di quei nodi metti **Color Space** su **Non-Color**. Vale per l'immagine, quindi basta una volta. Una normal map non è un colore: sono direzioni, e non vanno corrette come i colori.
4. Nel riquadro **Bake** scegli **Bake Type: Normal**. **Space** resta **Tangent**. Premi **Bake**.
5. Torna alla scheda **UV Editing**: l'editor a sinistra mostra `cassetta_mista_atlas_normal`. **Image ▸ Save As**, PNG, stessa cartella, nome `cassetta_mista_atlas_normal.png`.
> lavoro: 5 3

## In Blender · un materiale solo
1. Nella scheda **Object Data**, in **UV Maps**, clicca `UVMap` e premi **−**. Resta solo `Atlas`, ora con la fotocamera.
2. Nella scheda **Material** clicca il secondo materiale e premi **−** quattro volte. Resta un materiale solo, quello del frutto che avevi cliccato. Fai doppio clic sul suo nome e chiamalo `Cassetta atlas`.
3. Nell'editor dei nodi trovi il nodo della texture del frutto collegato a **Base Color**: nel suo menu scegli `cassetta_mista_atlas`. Nel nodo collegato al **Normal Map** scegli `cassetta_mista_atlas_normal`. Il nodo che avevi posato per il bake ora non serve: selezionalo e premi **X**.
4. Col mouse sulla vista 3D premi **Z** e scegli **Material Preview**: i cinque frutti devono sembrare quelli di prima.
> lavoro: 6 4

## In Blender · esporta
1. Con il frutto unito selezionato, **File ▸ Export ▸ FBX (.fbx)**.
2. A destra, sotto **Include**, spunta **Selected Objects**. Sotto **Transform**, in **Apply Scalings**, scegli **FBX Units Scale**.
3. Salva sopra `cassetta_mista.fbx`, nella cartella `sandbox/Assets/Demo/Models/Mercato`, con lo stesso nome.
Se in Unity le cassette diventano minuscole, hai dimenticato **FBX Units Scale**: rifai l'esportazione. Se qualcosa è andato storto in Blender, ricomincia in un file nuovo: **File ▸ New ▸ General**, e importa di nuovo il modello. Se l'hai già sovrascritto, nella cartella della tua copia lancia `git checkout -- sandbox/Assets/Demo/Models/Mercato/cassetta_mista.fbx` e il modello torna com'era.
> lavoro: 3 2

## In Unity · il materiale dell'atlas
Torna in Unity. Le cassette si aggiornano da sole: ora ognuna ha un oggetto solo al posto dei cinque frutti, ma con un materiale che non va bene.
1. Nella finestra **Project** apri `Assets/Demo/Models/Mercato` e seleziona `cassetta_mista_atlas_normal`. Nell'Inspector metti **Texture Type** su **Normal map** e premi **Apply**.
2. Nella stessa cartella clicca col destro e scegli **Create ▸ Material**. Chiamalo `Cassetta atlas`.
3. Nell'Inspector del materiale, nel menu **Shader**, scegli **Universal Render Pipeline ▸ Simple Lit**. Trascina `cassetta_mista_atlas` sul quadratino di **Base Map** e `cassetta_mista_atlas_normal` su quello di **Normal Map**. Sotto **Advanced Options** metti la spunta a **Specular Highlights**.
4. Apri `Assets/Samples/RUFA Core/Lezione 07 - Draw call/Prefab` e fai doppio clic su `Cassetta mista`. Nella Hierarchy apri `Cassetta mista` e seleziona il frutto unito: si chiama come il frutto che avevi cliccato in Blender, per esempio `Frutto 2`.
5. Nell'Inspector, nel **Mesh Renderer**, alla voce **Materials**, trascina `Cassetta atlas` sopra il materiale che c'è.
6. Esci dal prefab con la freccia in alto a sinistra nella Hierarchy.
Se la pagina resta su **Da fare**: il modello ha ancora più parti (in Blender manca l'unione, o un materiale), oppure il frutto unito ha ancora un materiale con una texture vecchia, o due materiali.
> verifica: un-materiale Assets/Demo/Models/Mercato/cassetta_mista.fbx
> lavoro: 6 4

## Le cassette · il dopo
Ora che le cassette hanno un materiale solo, lo static batching le può mettere insieme.
1. Nella Hierarchy apri `Banco delle cassette` e seleziona `Cassette`. Nell'Inspector apri la freccia di **Static**, scegli **Batching Static** e rispondi **Yes, change children**.
2. Premi **Play**, scegli **Display 6** e rileggi i numeri. Scrivili nel file ed esci dal Play.
3. Fai la foto del dopo e mostrala al curatore insieme a quella del prima.
4. Salva la scena con **Ctrl+S**.
> azione: Foto delle cassette, dopo | foto-camera | Camera Cassette | Consegne/L07/cassette-dopo.jpg
> verifica: file-esiste Consegne/L07/cassette-dopo.jpg

## Trucchi del mestiere · nel programma 3D
- **Un oggetto, un materiale.** Ogni materiale in più su un oggetto è una draw call in più. Quando puoi, fai un materiale solo e metti le differenze nella texture.
- **La palette.** Per i modelli a colori pieni basta una texture piccola con dei quadretti di colore: ogni isola UV si posa sul suo colore. Tanti modelli, un materiale.
- **Le trim sheet.** Una texture a strisce, ognuna con un dettaglio che si ripete: una cornice, un bordo di metallo, una fila di mattoni. Le pareti e i mobili di una stanza ci posano sopra le loro isole e condividono un materiale.
- **Unisci ciò che sta sempre insieme.** Le gambe e il piano di un tavolo sì. Un oggetto che si muove, si raccoglie o si spegne no: deve restare separato.
- **La densità dell'atlas.** Dai a ogni isola uno spazio proporzionato a quanto si vedrà. Un litchi non ha bisogno di mezzo atlas.
- **I margini.** Lascia qualche pixel fra le isole e cuoci con un margine: da lontano Unity usa versioni più piccole della texture, e senza margine i bordi prendono il colore del vicino.
- **Salva le immagini cotte.** Blender non le salva da solo.
- **Le normal map.** In **Non-Color**, sempre. Blender e Unity usano la stessa convenzione, quella di OpenGL, col verde verso l'alto. Una normal map fatta per DirectX ha il verde al contrario: il rilievo sembra scavato, e in Unity si corregge invertendo il verde.
> lavoro: 3 2

## Trucchi del mestiere · in Unity
- **Prima il Frame Debugger, poi la cura.** Ti dice quali draw call ci sono e perché un gruppo si interrompe.
- **Pochi shader, con le stesse opzioni.** Un Simple Lit per quasi tutto. Ogni opzione accesa su un materiale solo è una variante in più.
- **`sharedMaterial`, non `material`.** In uno script, `material` crea una copia del materiale per quell'oggetto, e la copia rompe lo static batching. Per cambiare il materiale di tutti usa `sharedMaterial`.
- **Niente `MaterialPropertyBlock` con l'SRP Batcher.** Lo interrompe. Unity consiglia materiali diversi per proprietà diverse.
- **Instancing spento.** Con l'SRP Batcher acceso non serve e aggiunge varianti.
- **Statico solo ciò che è fermo.** Un oggetto che si afferra, una porta che si apre, un cassetto: non vanno segnati **Batching Static**, perché uniti agli altri non si muovono più.
- **Le luci con le ombre.** Ogni luce che fa ombra ridisegna gli oggetti che la ricevono, e le draw call si moltiplicano. Lo vediamo nella lezione 8.
- **La misura vera è sul visore.** Stats sul PC conta le passate del PC. Per i numeri del Quest servono il Profiler collegato al visore e OVR Metrics, come nella lezione 5.
> lavoro: 3 2

# Da soli · Verifica

## I tre banchi
Tre banchi, tre budget, scritti sulla lavagna di ogni bottega. Per ogni banco il giro è lo stesso della frutta:
1. in Play, il Display del banco, e i numeri nel file delle misure;
2. la foto del prima;
3. la cura, fuori dal Play;
4. di nuovo in Play i numeri nel file, e la foto del dopo.
Le regole del curatore valgono sempre. Non spegnere, non cancellare e non spostare la merce: un mercato senza frutta non è un mercato ottimizzato. E la foto del dopo deve sembrare quella del prima.
Se hai fatto la cura e un compito resta **Da fare**, nel banco qualcosa è stato spostato o spento: annulla con **Ctrl+Z**, o chiamami. Alla fine di ogni banco salva con **Ctrl+S**.
La lista controlla la scena aperta: deve essere il mercato.
> verifica: scena-aperta Mercato
> soluzione: Se è aperta un'altra scena, nella finestra **Project** apri `Assets/Samples/RUFA Core/Lezione 07 - Draw call` e fai doppio clic su `Mercato`.

## Le spezie · il prima
Le spezie sono sul **Display 3**. In Play leggi i numeri e scrivili nel file. Esci dal Play e fai la foto.
> azione: Foto delle spezie, prima | foto-camera | Camera Spezie | Consegne/L07/spezie-prima.jpg
> verifica: file-esiste Consegne/L07/spezie-prima.jpg
> soluzione: Premi **Play**, nella Game view scegli **Display 3**, leggi **Batches**, **Saved by batching** e **SetPass calls** e scrivili nel file. Esci dal Play e premi il pulsante della foto.

## Le spezie · le copie
Ogni mucchio e ogni ciotola hanno la loro copia del materiale. Fondile: un materiale per spezia, uno per le ciotole.
> verifica: materiali-al-massimo Spezie | 11
> soluzione: Nella Hierarchy cerca `Paprika`, seleziona tutti i risultati e trascina il materiale `Paprika` (senza numero) della cartella `Materiali/Spezie` sul campo del materiale nell'Inspector. Fai lo stesso con `Curcuma`, `Cannella`, `Pepe nero`, `Zafferano`, `Curry`, `Sale rosa`, `Coriandolo`, `Sommacco`, `Pepe verde` e `Ciotola`. Svuota la ricerca e salva.

## Le spezie · uno shader
Le spezie usano shader e opzioni diversi. Mettile tutte su Simple Lit, con le stesse opzioni. Sono polveri opache: niente riflessi.
> verifica: una-variante Spezie | Universal Render Pipeline/Simple Lit
> soluzione: Nella cartella `Materiali/Spezie` seleziona tutti i materiali con **Ctrl+A**. **Shader ▸ Universal Render Pipeline ▸ Simple Lit**. Sotto **Advanced Options** togli la spunta a **Specular Highlights** e a **Enable GPU Instancing**: se una casella mostra un trattino, cliccala finché è vuota. Le ciotole non hanno la normal map, e nemmeno i mucchi: così sono tutti uguali.

## Le spezie · ferme
> verifica: statici Spezie
> soluzione: Nella Hierarchy seleziona `Spezie`, dentro `Banco delle spezie`. Nell'Inspector apri la freccia di **Static**, scegli **Batching Static** e rispondi **Yes, change children**. Salva.

## Le spezie · il dopo
In Play, sul **Display 3**, rileggi i numeri e scrivili nel file. Esci dal Play e fai la foto del dopo.
> azione: Foto delle spezie, dopo | foto-camera | Camera Spezie | Consegne/L07/spezie-dopo.jpg
> verifica: file-esiste Consegne/L07/spezie-dopo.jpg
> soluzione: Premi **Play**, **Display 3**, scrivi i numeri, esci dal Play e premi il pulsante della foto.

## La vetrina · il prima
La pasticceria è sul **Display 4**. Il budget qui non è in draw call: il cliente vuole al massimo tre strati di vetro uno dietro l'altro.
1. In Play scegli **Display 4**, leggi i numeri e scrivili nel file.
2. Apri il **Rendering Debugger**, **Rendering ▸ Overdraw Mode: Transparent**, e guarda quanto si accende la vetrina. Rimetti **None** ed esci dal Play.
3. Fai la foto.
> azione: Foto della vetrina, prima | foto-camera | Camera Vetrina | Consegne/L07/vetrina-prima.jpg
> verifica: file-esiste Consegne/L07/vetrina-prima.jpg
> soluzione: Premi **Play**, **Display 4**, scrivi i numeri. **Window ▸ Analysis ▸ Rendering Debugger**, **Rendering**, **Overdraw Mode: Transparent**: la vetrina è molto chiara. Rimetti **None**, esci dal Play e premi il pulsante della foto.

## La vetrina · i vetri
Ogni lastra della vetrina è doppia, il ripiano di vetro pure, e ogni campana sopra le torte ne ha un'altra dentro. Togli gli strati che non servono, senza toccare i dolci.
> verifica: trasparenti-fra Vetri della vetrina | 6 | 9 | Vetrina
> soluzione: Nella Hierarchy apri `Pasticceria` e poi `Vetri della vetrina`. Seleziona tutti gli oggetti che finiscono con **interna**: le cinque lastre, il ripiano e le tre campane. Premi **Canc**. Il materiale `Vetro` è lo stesso della vetrinetta della frutta: se lì hai già portato **A** a 23, vale anche qui; se no, fallo ora (il colore accanto a **Base Map**, **A** da 12 a 23). Salva.

## La vetrina · il dopo
In Play, sul **Display 4**, rileggi i numeri, riguarda l'overdraw e scrivi nel file cosa è cambiato. Esci dal Play e fai la foto del dopo.
> azione: Foto della vetrina, dopo | foto-camera | Camera Vetrina | Consegne/L07/vetrina-dopo.jpg
> verifica: file-esiste Consegne/L07/vetrina-dopo.jpg
> soluzione: Premi **Play**, **Display 4**, scrivi i numeri, controlla l'overdraw nel Rendering Debugger, esci dal Play e premi il pulsante della foto.

## Il bar · il prima
Il bar è sul **Display 5**. In Play leggi i numeri e scrivili nel file. Esci dal Play e fai la foto.
> azione: Foto del bar, prima | foto-camera | Camera Bar | Consegne/L07/bar-prima.jpg
> verifica: file-esiste Consegne/L07/bar-prima.jpg
> soluzione: Premi **Play**, **Display 5**, scrivi i numeri, esci dal Play e premi il pulsante della foto.

## Il bar · le copie
Ogni sedia e ogni tavolino hanno la loro copia del materiale. Fondile: un materiale per le sedie, uno per i tavolini.
> verifica: materiali-al-massimo Bar | 2
> soluzione: Nella Hierarchy cerca `Sedia`, seleziona tutti i risultati e trascina il materiale `Sedia` (senza numero) della cartella `Materiali/Bar` sul campo del materiale nell'Inspector. Fai lo stesso con `Tavolino`. Svuota la ricerca e salva.

## Il bar · l'Instancing alla prova
Le sedie sono tutte uguali: sembra il caso giusto per l'Instancing. Provalo.
1. Nella cartella `Materiali/Bar` seleziona il materiale `Sedia`. Sotto **Advanced Options** metti la spunta a **Enable GPU Instancing**.
2. Premi **Play**, **Display 5**, e leggi **Batches**: non cambia niente. Con l'SRP Batcher acceso, Unity non usa l'Instancing.
3. Esci dal Play e togli la spunta. Nel file scrivi cosa hai visto.
> lavoro: 3 2

## Il bar · fermi
La cura vera è lo static batching: sedie e tavolini non si muovono.
> verifica: statici Bar
> soluzione: Nella Hierarchy seleziona `Bar`, dentro `Bar del mercato`. Nell'Inspector apri la freccia di **Static**, scegli **Batching Static** e rispondi **Yes, change children**. Salva.

## Il bar · il dopo
In Play, sul **Display 5**, rileggi i numeri e scrivili nel file. Esci dal Play e fai la foto del dopo.
> azione: Foto del bar, dopo | foto-camera | Camera Bar | Consegne/L07/bar-dopo.jpg
> verifica: file-esiste Consegne/L07/bar-dopo.jpg
> soluzione: Premi **Play**, **Display 5**, scrivi i numeri, esci dal Play e premi il pulsante della foto.

## Il curatore
Come alla frutta: il compagno fa il curatore sulle tue foto dei tre banchi, e tu sulle sue. Per ogni banco gli mostri le due foto a tutto schermo, nell'ordine che decidi tu, senza fargli vedere i nomi dei file. Se ti indica una differenza che c'è davvero, torna al banco e regola: la lucentezza di un materiale, l'**Alpha** del vetro. Scrivi nel file delle misure cosa ha notato e cosa hai cambiato.
> lavoro: 4 3

# Da soli · Il tuo progetto

## La tua scena
Ora il tuo progetto. Premi il pulsante: si apre la scena del tuo blockout.
> azione: Apri la tua scena | crea-progetto
> verifica: scena-aperta Progetto

## La camera
Nella lezione 6 hai messo nella tua scena una camera fissa, `Camera foto`, sul **Display 2**. Usa quella. Se non c'è, rifalla:
1. Nella Scene view inquadra il punto che conta di più, all'altezza degli occhi.
2. **GameObject ▸ Camera**, e rinominala `Camera foto`.
3. Con la camera selezionata, **GameObject ▸ Align With View**.
4. Nell'Inspector della camera metti **Target Display** su **Display 2**.
> verifica: scena-componente Camera foto | Camera
> lavoro: 3 1

## Il prima
1. Premi **Play**, scegli **Display 2** e leggi **Batches**, **Saved by batching** e **SetPass calls**.
2. Scrivili nel file delle misure, nella parte del progetto. Esci dal Play.
3. Fai la foto.
> azione: Foto del progetto, prima | foto-camera | Camera foto | Consegne/L07/progetto-prima.jpg
> verifica: file-esiste Consegne/L07/progetto-prima.jpg

## La cura
Usa le cure di oggi dove servono:
- con il Frame Debugger, trova i materiali e gli shader che interrompono i gruppi;
- un materiale solo per gli oggetti uguali, e niente copie;
- un solo shader mobile, Simple Lit, con le stesse opzioni;
- **Batching Static** su tutto quello che non si muove, e su niente di quello che si afferra o si apre;
- meno strati trasparenti, e niente alpha test dove puoi;
- se modelli, un atlas per i tuoi oggetti fatti di pezzi.
Il budget del tuo progetto è quello di Meta per il tuo tipo di gioco. Scrivi nel file cosa hai fatto e perché.
> lavoro: 25 15

## Il dopo
1. In Play, sul **Display 2**, rileggi i numeri e scrivili nel file. Esci dal Play.
2. Fai la foto del dopo.
3. Salva la scena con **Ctrl+S**.
> azione: Foto del progetto, dopo | foto-camera | Camera foto | Consegne/L07/progetto-dopo.jpg
> verifica: file-esiste Consegne/L07/progetto-dopo.jpg

## Le misure
Controlla che nel file delle misure non resti nessun «da scrivere».
> verifica: file-compilato Consegne/L07/misure.md | «da scrivere»
> lavoro: 5 3

## A fine lezione
Salva il lavoro. Nella cartella della tua copia lancia:
`git salva "Lezione 07"`
