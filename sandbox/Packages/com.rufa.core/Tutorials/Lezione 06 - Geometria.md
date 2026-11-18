Lezione 6 · La geometria nel museo. Le note prima del primo capitolo non si vedono nella finestra. Capitoli insieme 0-4 al massimo 90' per lo studente lento, il laboratorio (capitolo 5) al massimo 60', verifica al massimo 45' (tools/stima-tempi.py, righe dei capitoli e "Verifica"). Parla Gianluca in prima persona e dà del tu allo studente. Nessun numero misurato su un PC: solo quelli di Meta e di Unity, e quelli costruiti nella scena (i budget dei cartelli, il pavimento da 980.000 triangoli, la sfera da un milione, i modelli di Poly Haven). Si misura fuori dal Play, con le camere fisse sui Display 2-5 della Game view.

# 0 · Prima di iniziare

## Cosa farai in questa lezione
Oggi lavori per un cliente. Un museo vuole la sua mostra in realtà virtuale, sul Quest 3S. Le opere ci sono già: statue, busti e oggetti scaricati da Poly Haven così come sono, a piena risoluzione, come farebbe chiunque.
Una per una vanno bene. Tutte insieme il visore non ce la fa.
Il tuo lavoro è far stare il museo nel budget del cliente senza che il museo cambi aspetto. Il salone lo sistemiamo insieme, un pezzo per volta. Dopo la pausa, nel laboratorio di restauro, prepari le opere nuove in Blender, come fa chi modella. Poi tre sale le sistemi da solo, e un compagno farà il curatore: guarderà le tue foto del prima e del dopo e proverà a indovinare quale è quella ottimizzata.
È un lavoro che ti capiterà davvero. Chi fa avatar per VRChat ha un limite di triangoli per gli avatar che girano su Quest. Roblox fissa un massimo di triangoli per ogni accessorio. L'Asset Store di Unity vuole il numero dei poligoni nella descrizione di ogni modello. Un budget di triangoli è una richiesta normale di un cliente.

## I modelli nuovi
Questa pagina controlla che nel tuo progetto ci siano le opere del museo, in `Assets/Demo/Models/Museo`.
Se qui sotto c'è scritto **Fatto**, vai avanti. Se resta **Da fare**, chiudi Unity, lancia `git aggiorna` un'altra volta nella cartella della tua copia e riapri il progetto. Il primo `git aggiorna` di oggi porta il comando nuovo, il secondo porta i file. Unity Hub deve essere aperto: il progetto deve riscaricare i suoi package.
> verifica: asset-esiste Assets/Demo/Models/Museo/marble_bust_01.fbx

## Apri il museo
Premi il pulsante qui sotto. Unity copia il museo nel tuo progetto, in `Assets/Samples/RUFA Core/Lezione 06 - Geometria`, e apre la scena `Museo`. Il laboratorio e il deposito hanno una scena loro, `Laboratorio` e `Deposito`: li apri più avanti.
La copia è tua: puoi cambiarla quanto vuoi. Se la rovini, cancella quella cartella e premi di nuovo il pulsante.
> azione: Apri il museo | apri-scena | Lezione 06 - Geometria | Museo
> verifica: scena-aperta Museo

## Le quattro camere
Il museo ha quattro parti: il salone, il giardino, il deposito e la sala delle curiosità. Ognuna ha una camera fissa all'ingresso, dove entra il visitatore. Tutte le misure di oggi le leggi da quelle camere, fuori dal Play.
1. Clicca la linguetta **Game** per mettere la Game view davanti.
2. In alto a sinistra nella Game view c'è un menu con scritto **Display 1**. Aprilo e scegli **Display 2**: compare il salone, visto dalla sua camera.
3. Accanto c'è il menu delle proporzioni, quello con scritto **Free Aspect**. Scegli **16:9 Aspect**. Così l'inquadratura è la stessa su tutti i PC.
4. In alto a destra nella Game view premi **Stats**. Compare un riquadro coi numeri del frame.
Il giardino è sul **Display 3**, le curiosità sul **Display 5**. Il deposito, nella sua scena, sul **Display 4**.
> lavoro: 2 1

## La tua opera in cornice
Nel salone, sul cavalletto vicino all'ingresso, c'è una tela bianca. È per te. Se hai un tuo disegno, un tuo render o una tua foto, appendila.
1. Trascina il file della tua immagine, PNG o JPG, dall'Esplora file alla cartella `Assets/Progetto` nella finestra **Project**.
2. Nella Hierarchy apri `Salone`, poi `Cavalletto`, poi `La tua opera`, e seleziona `Tela`.
3. In fondo all'Inspector c'è il materiale `Tela`. Aprilo con la freccia. Sotto **Surface Inputs**, accanto a **Base Map**, c'è un quadratino: trascinaci sopra la tua immagine.
Se oggi non hai un'immagine con te, salta questa pagina. La tela resta bianca.
> lavoro: 3 2

## Il file delle misure
Oggi scrivi i numeri che leggi in un file, `Consegne/L06/misure.md`. Premi il pulsante: il file si crea e si apre nel blocco note. Tienilo aperto, ci torni spesso.
> azione: Crea il file delle misure | crea-misure | Consegne/L06/misure.md
> verifica: file-esiste Consegne/L06/misure.md

# 1 · Il budget

## Quanto disegna un Quest 3S
Un **triangolo** è il pezzo più piccolo di un modello 3D. Ogni superficie che vedi in un gioco è fatta di triangoli, anche quelle curve. Il visore deve trasformare ogni triangolo, cioè calcolare dove cade sullo schermo, e poi colorarlo. In VR lo fa per due occhi.
Meta dice quanti triangoli può disegnare il Quest 3 e il 3S in un frame: da 1,3 a 1,8 milioni. È un totale per frame, non per oggetto. Dentro ci deve stare tutto: le pareti, il pavimento, le opere, i personaggi, gli effetti e l'interfaccia.
Il budget non lo decide il modellatore da solo. Lo dà il cliente, o lo studio, prima di cominciare. Il nostro cliente l'ha scritto su un cartello all'ingresso di ogni sala.

## Cosa conta Stats
Nel riquadro **Stats** il numero da guardare oggi è **Tris**: i triangoli che Unity disegna in quel frame.
Conta solo quello che la camera disegna, non tutto quello che c'è nella scena. Però conta ogni passata: se lo stesso oggetto viene disegnato due volte, i suoi triangoli contano due volte. Quando l'editor è impostato per il PC, il nostro progetto disegna la scena una volta in più, per l'**occlusione ambientale** (SSAO), cioè l'ombra morbida negli angoli che ha la versione PC. Per questo vedrai numeri più alti dei triangoli dei modelli. Se invece hai lasciato l'editor su Android dopo una build per il visore, questa passata non c'è e i numeri sono più bassi, come sul visore.
I budget sui cartelli sono pensati per l'editor impostato per il PC. Su Android hai solo più margine.

## Il prima del salone
1. Nella Game view scegli **Display 2**: il salone.
2. Leggi **Tris** nel riquadro Stats.
3. Scrivilo nel file delle misure, alla riga del salone.
4. Confrontalo col budget sul cartello. È molto più alto.
Ora fai la foto del prima. La foto è un'immagine di quello che vede la camera del salone, come uno screenshot. Il pulsante la salva in un file, `salone-prima.jpg`, nella cartella `Consegne/L06` della tua copia, accanto a `sandbox`: la stessa cartella del file delle misure.
> azione: Foto del salone, prima | foto-camera | Camera Salone | Consegne/L06/salone-prima.jpg
> verifica: file-esiste Consegne/L06/salone-prima.jpg

# 2 · I triangoli che non si vedono

## Piatto vuol dire due triangoli
Una superficie piatta ha bisogno di due triangoli, che sia un quadretto o un pavimento di cento metri. Ogni triangolo in più su una superficie piatta non si vede, ma costa.
Succede spesso quando si esporta da un programma 3D con le suddivisioni ancora attive. Il pavimento del salone è così: 980.000 triangoli dove ne bastano due.
Meta lo dice in un altro modo: il dettaglio messo dove non si nota toglie risorse a quello che si nota.

## Il pavimento
1. Clicca la linguetta **Scene** per vedere la Scene view.
2. Nella barra in alto della Scene view apri il menu della modalità di disegno, quello con l'icona a sfera, e scegli **Wireframe**. Ora vedi i triangoli come linee.
3. Nella Hierarchy apri `Salone` e seleziona `Pavimento`. È talmente fitto che sembra pieno.
4. Nell'Inspector, nel componente **Dense Grid**, scrivi `1` in **Cells** e premi Invio. Il pavimento ora ha due triangoli.
5. Rimetti la Scene view su **Shaded** e torna alla Game view: il pavimento è identico. Leggi **Tris**: è crollato.
Nel tuo programma 3D è come togliere le suddivisioni, o usare il Decimate in modalità **Planar**, che unisce le facce piatte.
> verifica: scena-numero Pavimento | DenseGrid | cells | 1 | 4

## Il dettaglio nella texture
Il legno del pavimento ha venature e fessure. Non sono triangoli: sono nella **normal map**, una texture che dice alla luce come è inclinata la superficie punto per punto. La luce crede che ci siano rilievi che non ci sono.
Meta consiglia di mettere il dettaglio nelle texture, non nei triangoli. Prova:
1. Seleziona di nuovo `Pavimento`. In fondo all'Inspector apri il materiale `Parquet`.
2. Sotto **Surface Inputs**, accanto a **Normal Map**, c'è un numero: 1. Scrivi `0` e guarda il pavimento nella Game view, soprattutto lontano, dove lo vedi di taglio. Il legno diventa liscio come plastica.
3. Rimetti `1`.
Il rilievo non è costato nemmeno un triangolo.
> lavoro: 3 2

## La scultura
La sfera al centro è uscita così da un programma di scultura, come ZBrush: un milione di triangoli. Qui la regola è un'altra: la sfera non è piatta, quindi qualche triangolo serve. Quanti? Lo decide il tuo occhio.
1. Seleziona `Sfera`. Nell'Inspector, nel componente **Dense Mesh**, c'è **Triangles**: 1000000.
2. Abbassalo un passo alla volta: `100000`, poi `20000`, poi `5000`, poi `1000`, e ogni volta guarda la Game view e la Scene view da vicino.
3. Quando vedi gli spigoli sul bordo della sfera sei sceso troppo. Torna al valore di prima.
Nel tuo programma 3D lo stesso lavoro si fa col Decimate, o rifacendo la rete del modello con meno poligoni, la **retopologia**.
> verifica: scena-numero Sfera | DenseMesh | triangles | 8 | 20000

## Rileggi i triangoli
Torna alla Game view sul **Display 2** e rileggi **Tris**. Scrivilo nel file delle misure, accanto al primo numero del salone.
Siamo scesi molto. Il budget però non c'è ancora: restano le opere.
> lavoro: 1 1

# 3 · Il LOD

## Lontano basta meno
Un busto in fondo alla sala occupa pochi pixel dello schermo. Disegnarlo con tutti i suoi diciassettemila triangoli è uno spreco.
Il **LOD**, livello di dettaglio, risolve così: dello stesso modello ci sono più versioni, sempre più semplici, e Unity sceglie quale disegnare secondo quanto è grande l'oggetto sullo schermo. In Unity 6.3 ci sono due modi per farlo. Cambia chi prepara le versioni e quanto le controlli.
Il **LOD Group** è un componente. Le versioni semplificate le prepara chi modella, come modelli separati: le nostre le ho fatte in Blender col Decimate, al 50% e al 15% dei triangoli. Il componente ha delle fasce: **LOD 0** è il modello pieno, **LOD 1** e **LOD 2** sono le versioni semplificate, **Culled** vuol dire che l'oggetto non si disegna più. Ogni fascia ha una soglia, la percentuale dell'altezza dello schermo che l'oggetto occupa: al 50% l'oggetto riempie metà dello schermo in altezza. Le soglie le sposti tu, e vedi quale livello sta usando.
Quando il modello passa da un livello all'altro, a volte si vede un piccolo salto. Si chiama **popping**, e si corregge spostando le soglie.
Il **Mesh LOD** invece è un'opzione dell'importazione: nell'Inspector del modello, nella sezione **Mesh LODs**, la casella **Generate mesh LODs**. Unity semplifica il modello da solo e mette i livelli dentro la mesh stessa, senza oggetti in più. È comodo e occupa meno memoria. Però non decidi tu come viene semplificato, non vedi quale livello sta usando, e con lo static batching, che vediamo nella lezione 7, usa sempre il modello pieno. Unity sconsiglia di usarlo insieme al LOD Group.
Oggi usiamo il LOD Group: le versioni e le soglie le controlli tu, e vedi cosa succede.

## I busti
I dodici busti della galleria sono copie di un solo **prefab**, `Busto`. Se cambi il prefab, cambiano tutte le copie.
1. Nella finestra **Project** apri `Assets/Samples/RUFA Core/Lezione 06 - Geometria/Prefab` e fai doppio clic su `Busto`. Il prefab si apre da solo, su fondo blu.
2. Nella Hierarchy seleziona `Busto`, l'unico oggetto.
3. Dal menu in alto scegli **RUFA ▸ Strumenti ▸ LOD Group dalle varianti selezionate**. Lo strumento cerca accanto al modello le varianti `_LOD1` e `_LOD2`, le aggiunge come figli e crea il LOD Group.
4. Esci dal prefab con la freccia a sinistra in alto nella Hierarchy. Il prefab si salva da solo.
Se la pagina resta su **Da fare**, nel salone qualcosa è stato spostato o spento: annulla con **Ctrl+Z**, o chiamami.
> verifica: lod-su Salone | Busto

## Le fasce
1. Nella Hierarchy apri `Salone` e seleziona `Busto 1`.
2. Nell'Inspector, nel **LOD Group**, ci sono le fasce colorate con le loro percentuali.
3. Sopra le fasce c'è una piccola camera. Trascinala a destra e a sinistra: nella Scene view il busto cambia livello.
4. Metti la Scene view in **Wireframe** e allontanati dal busto con la rotella del mouse: le linee si diradano a ogni salto.
Se un salto si nota troppo, trascina il confine fra due fasce. Ricorda che una modifica sulla copia vale solo per quella copia: le soglie per tutti i busti si cambiano nel prefab.
> lavoro: 4 2

## Rileggi i triangoli
Torna alla Game view sul **Display 2**, rileggi **Tris** e scrivilo nel file.
> lavoro: 1 1

# 4 · Il culling

## Fuori dall'inquadratura e dietro i muri
**Culling** vuol dire scartare quello che non serve disegnare.
Quello che sta fuori dall'inquadratura Unity lo scarta da solo, a ogni frame, senza costo per te. Si chiama **frustum culling**.
Quello che sta dietro un muro invece Unity lo disegna lo stesso, anche se il muro lo copre tutto. Per scartarlo bisogna calcolare prima, una volta, chi nasconde chi. È l'**occlusion culling**, e il calcolo si chiama **bake**.
Il bake usa solo gli oggetti che non si muovono, segnati come **statici**. Un oggetto **Occluder Static** nasconde quello che ha dietro; un oggetto **Occludee Static** può essere nascosto. Un oggetto che si muove può essere nascosto, ma non nascondere.
Meta lo consiglia per gli ambienti chiusi, con muri grandi. In uno spazio aperto o in una scena piccola non serve.
Dietro i muri lunghi del salone ci sono due salette piene di opere. La camera non le vede, ma il visore le disegna tutte.

## Le salette
1. Nella Hierarchy seleziona `Salone`, l'oggetto in cima.
2. Dal menu in alto scegli **RUFA ▸ Strumenti ▸ Statici: imposta flag (selezione)**. Tutti gli oggetti del salone diventano statici, e i vetri non nascondono niente. Nella Console compare `renderers marked static`.
3. Dal menu in alto scegli **RUFA ▸ Strumenti ▸ Occlusion culling: bake**. Unity fa il calcolo tutto in una volta e per un po' resta fermo: aspetta che torni a rispondere.
Se la pagina resta su **Da fare** e nella Console c'è `No Renderers that are marked static`, il passo 2 non è partito: rifallo con `Salone` selezionato, poi rifai il bake. Non confonderlo con **Lighting: bake**, che cuoce la luce e oggi non serve.
> verifica: occlusion-su Salone | Muri del salone
> attesa: 30 Il bake

## Cosa vede la camera
1. Dal menu in alto apri **Window ▸ Rendering ▸ Occlusion Culling** e scegli la scheda **Visualization**.
2. Nella Hierarchy seleziona `Camera Salone`, dentro `Salone`.
3. Nella Scene view spariscono gli oggetti che la camera non vede: le salette, tranne quello che si intravede dalle porte.
Se sposti un muro dopo il bake, rifai il bake: i dati vecchi nasconderebbero le cose sbagliate.
> lavoro: 3 2

## Il dopo del salone
1. Torna alla Game view sul **Display 2** e rileggi **Tris**. Ora è sotto il budget del cartello.
2. Scrivilo nel file delle misure.
3. Fai la foto del dopo.
4. Salva la scena con **Ctrl+S**.
> azione: Foto del salone, dopo | foto-camera | Camera Salone | Consegne/L06/salone-dopo.jpg
> verifica: file-esiste Consegne/L06/salone-dopo.jpg

## Il curatore
Nei musei il **curatore** è chi si occupa della mostra. Prima di accettare il tuo lavoro controlla una cosa sola: che il salone non sia cambiato. Il curatore lo fa il tuo compagno di banco, e tu lo fai per lui.
1. Apri la cartella `Consegne/L06` nell'Esplora file. Ci sono le due foto del salone, `salone-prima.jpg` e `salone-dopo.jpg`.
2. Decidi tu in che ordine mostrarle. Apri la prima con un doppio clic e mettila a tutto schermo, così il compagno non vede il nome del file: il nome dice quale è il prima. Poi fai lo stesso con l'altra.
3. Il compagno dice quale delle due è quella ottimizzata, e perché: cosa vede di diverso.
Se ti indica una differenza che c'è davvero, torna nel salone e correggila. Se non trova differenze, o tira a indovinare, hai fatto un buon lavoro.
Poi scambiatevi i ruoli: tu fai il curatore sulle sue foto.
> lavoro: 4 3

# 5 · Il laboratorio

## Il laboratorio di restauro
In un museo, prima di arrivare in sala, ogni opera passa dal laboratorio di restauro: lì si prepara.
Con i modelli 3D è uguale. Gran parte dell'ottimizzazione si fa prima di Unity, nel programma 3D, da chi modella. È il lavoro che un cliente si aspetta quando ti dà un budget.
Nel laboratorio ci sono quattro postazioni:
- in fondo, **le nicchie**: tre busti che nessuno vede da dietro;
- a sinistra, **il tuo LOD**: cinque vasi d'ottone senza versioni semplificate;
- a destra, **il Mesh LOD**: cinque vasi antichi alle stesse distanze, per il confronto;
- davanti, **il vaso sfaccettato**: un modello importato male.
Premi il pulsante. Unity ti chiede se salvare il museo: rispondi **Save**. Nella Game view la camera dell'ingresso è sul **Display 6**, quella delle nicchie sul **Display 7**.
> azione: Apri il laboratorio | apri-scena | Lezione 06 - Geometria | Laboratorio
> verifica: scena-aperta Laboratorio

## Le facce che nessuno vede
Ogni faccia ha un davanti e un dietro. La GPU scarta le facce che le girano le spalle: si chiama **backface culling**. Però le scarta solo dopo aver calcolato i loro vertici: una faccia che nessuno vede costa comunque, e Stats la conta nei **Tris**.
Pensa a dove starà il modello e da dove lo guarderà il visitatore:
- un busto in una nicchia si vede solo da davanti: tutto il retro, dalla nuca alla base, non lo vede nessuno;
- un mobile contro il muro: il retro e il fondo non si vedono mai;
- un vaso su un piedistallo: il fondo appoggiato non si vede;
- due pezzi infilati uno nell'altro, come le gambe di una sedia nel sedile: le parti dentro non si vedono.
Quelle facce si cancellano nel programma 3D. Il visitatore non se ne accorge, il visore sì.
> lavoro: 2 1

## Le nicchie · il prima
1. Nella Game view scegli **Display 7**: le nicchie.
2. Leggi **Tris** e scrivilo nel file delle misure, alla riga delle nicchie.
3. Fai la foto del prima.
> azione: Foto delle nicchie, prima | foto-camera | Camera delle nicchie | Consegne/L06/nicchie-prima.jpg
> verifica: file-esiste Consegne/L06/nicchie-prima.jpg

## Le nicchie · il retro in Blender
I tre busti sono copie dello stesso modello, `busto_nicchia.fbx`: lo cambi una volta e cambiano tutti e tre.
1. Apri Blender. Clicca il cubo, premi **X** e scegli **Delete**.
2. Dal menu **File ▸ Import ▸ FBX (.fbx)** apri `busto_nicchia.fbx`. Sta nella cartella della tua copia, in `sandbox/Assets/Demo/Models/Museo`.
3. Clicca il busto e premi **Tab**: sei in Edit Mode. Premi **3** per lavorare sulle facce e **Alt+Z** per vedere attraverso il modello.
4. Guarda il busto di lato: nel cerchio degli assi, in alto a destra della vista, clicca il pallino rosso **X**.
5. Trascina col mouse un rettangolo su tutta la parte dietro del busto, dalla cima della testa fino alla base, un dito dietro le orecchie. Le facce prese diventano arancioni.
6. Premi **X** e scegli **Faces**. Poi premi **Tab** per tornare in Object Mode.
7. **File ▸ Export ▸ FBX (.fbx)**. A destra, sotto **Include**, spunta **Selected Objects**; sotto **Transform**, in **Apply Scalings** scegli **FBX Units Scale**. Salva sopra `busto_nicchia.fbx`, con lo stesso nome.
Torna in Unity: i busti si aggiornano da soli. Se diventano minuscoli, hai dimenticato **FBX Units Scale**: rifai l'esportazione. Se hai tagliato troppo, ricomincia in un file nuovo: **File ▸ New ▸ General**, cancella il cubo, importa `marble_bust_01.fbx` della stessa cartella, che è lo stesso busto, ed esportalo sopra `busto_nicchia.fbx`.
Se la pagina resta su **Da fare**, hai tolto troppo poco: il rettangolo deve prendere tutto il retro, dalla testa alla base. Oppure nel laboratorio qualcosa è stato spostato: annulla con **Ctrl+Z**.
> verifica: retro-tolto Laboratorio | Busto nella nicchia | 10500 | 15000 | 0.45
> lavoro: 10 6

## Le nicchie · il dopo
1. Sul **Display 7** rileggi **Tris** e scrivilo nel file.
2. Fai la foto del dopo e mostrala al curatore insieme a quella del prima. Se vede un buco dietro le orecchie, hai tagliato troppo avanti: rifai il taglio con più margine.
> azione: Foto delle nicchie, dopo | foto-camera | Camera delle nicchie | Consegne/L06/nicchie-dopo.jpg
> verifica: file-esiste Consegne/L06/nicchie-dopo.jpg

## Le versioni le fai tu
I vasi d'ottone hanno 21.440 triangoli e nessuna versione semplificata: le fai tu, col **Decimate**, il modificatore di Blender che toglie facce. Ha tre modi:
- **Collapse** unisce i vertici poco alla volta e tiene la forma: va bene quasi sempre;
- **Un-Subdivide** toglie le suddivisioni a un modello fatto suddividendo;
- **Planar** semplifica le parti piatte.
**Ratio** dice quanto tenere: 0.5 vuol dire metà delle facce. Un numero giusto per tutti non c'è: scendi finché la sagoma regge. La sagoma è il contorno del modello, ed è la prima cosa che l'occhio nota, soprattutto da lontano.
1. In Blender scegli **File ▸ New ▸ General** (se chiede di salvare, **Don't Save**) e cancella il cubo.
2. **File ▸ Import ▸ FBX (.fbx)** e apri `brass_vase_01.fbx`, in `sandbox/Assets/Demo/Models/Museo`.
3. Clicca il vaso. A destra, nelle **Properties**, apri la scheda con la chiave inglese, **Modifiers**, e scegli **Add Modifier ▸ Generate ▸ Decimate**.
4. Scrivi un **Ratio**, per esempio 0.5, e guarda il vaso da vicino e da lontano con la rotella. Nel modificatore leggi quante facce restano.
5. **File ▸ Export ▸ FBX (.fbx)**, con **Selected Objects** e **Apply Scalings: FBX Units Scale**. Chiama il file `brass_vase_01_LOD1.fbx`, nella stessa cartella del vaso. Il Decimate lo applica l'esportazione: **Apply Modifiers** è già acceso.
6. Abbassa il **Ratio**, per esempio a 0.15, ed esporta di nuovo come `brass_vase_01_LOD2.fbx`.
7. In Unity, nella finestra **Project**, apri `Assets/Samples/RUFA Core/Lezione 06 - Geometria/Prefab` e fai doppio clic su `Vaso d'ottone`. Seleziona `Vaso d'ottone` nella Hierarchy e scegli **RUFA ▸ Strumenti ▸ LOD Group dalle varianti selezionate**. Esci con la freccia in alto nella Hierarchy.
8. Sul **Display 6** guarda la fila di sinistra: se un vaso cambia aspetto in modo visibile, riesporta quella versione con un Ratio più alto.
Se lo strumento dice che una versione ha un'altra grandezza, riesportala con **FBX Units Scale**. Se la pagina resta su **Da fare**, il LOD Group va sul prefab, non sui vasi della scena; oppure nel laboratorio qualcosa è stato spostato: **Ctrl+Z**.
> verifica: lod-su Laboratorio | Vaso d'ottone
> lavoro: 12 8

## Il Mesh LOD alla prova
Ora la stessa cosa la fa Unity da solo, sui vasi antichi della fila di destra.
1. Sul **Display 6** leggi **Tris**.
2. Nella finestra **Project** seleziona `Assets/Demo/Models/Museo/antique_ceramic_vase_01`.
3. Nell'Inspector, scheda **Model**, sezione **Mesh LODs**, spunta **Generate Mesh LODs** e premi **Apply** in fondo.
4. Rileggi **Tris** e confronta le due file: a sinistra le tue versioni, a destra quelle di Unity.
Unity ha fatto i livelli senza che tu toccassi niente. Però non decidi tu come semplifica, e non vedi quale livello sta usando. Nel file delle misure scrivi quale dei due terresti per questo vaso, e perché. Se la pagina resta su **Da fare**, manca **Apply**.
> verifica: mesh-lod Assets/Demo/Models/Museo/antique_ceramic_vase_01.fbx
> lavoro: 5 3

## Il vaso sfaccettato
Seleziona `Vaso sfaccettato` nella Hierarchy e, col mouse sulla Scene view, premi **F**: la vista ci si avvicina. Guarda il riflesso della luce: è fatto a quadretti, come se il vaso fosse di tante facce piatte. Ha gli stessi triangoli del vaso antico, ma quasi il triplo dei vertici.
Un vertice è condiviso dai triangoli che lo toccano solo se per tutti ha la stessa posizione, la stessa normale e le stesse UV. La **normale** è la direzione verso cui guarda la superficie in quel punto: su uno spigolo duro cambia di colpo, e il vertice si sdoppia. Lo stesso succede su una cucitura delle UV. In Stats, **Verts** conta i vertici.
Chi ha importato questo vaso ha reso duro ogni spigolo.
1. Sul **Display 6** leggi **Verts** e scrivilo nel file delle misure.
2. Nella finestra **Project** seleziona `Assets/Demo/Models/Museo/vaso_sfaccettato`.
3. Nell'Inspector, scheda **Model**, sezione **Geometry**: **Normals** è su **Calculate** e **Smoothness Source** su **From Angle**. Lo **Smoothing Angle** è 0: ogni spigolo è duro.
4. Alza lo **Smoothing Angle**, per esempio a 60, e premi **Apply**. Gli spigoli più morbidi di quell'angolo diventano lisci.
5. Guarda il vaso e rileggi **Verts**. Se la pagina resta su **Da fare**, alza ancora lo Smoothing Angle e premi di nuovo **Apply**.
Nel programma 3D è la stessa cosa: in Blender, **Object ▸ Shade Smooth** prima di esportare; da Blender 4.1 c'è anche **Shade Auto Smooth**, che lascia duri solo gli spigoli vivi.
> verifica: vertici-al-massimo Laboratorio | Vaso sfaccettato | 5500
> lavoro: 5 3

## Trucchi del mestiere · nel programma 3D
- **Il retro e il fondo.** Mobili contro il muro, oggetti appoggiati, busti nelle nicchie: le facce che toccano il muro o il pavimento si cancellano.
- **Le parti incastrate.** Se due pezzi entrano uno nell'altro, le facce dentro non si vedono: uniscili, o cancella quello che sta dentro.
- **Le facce doppie.** Un modello importato due volte, o due copie una sopra l'altra: costano il doppio e sfarfallano, perché la GPU non sa quale disegnare davanti. In Blender, in Edit Mode, **Mesh ▸ Clean Up ▸ Merge by Distance** le unisce.
- **Prima la sagoma.** I triangoli servono sul contorno, dove l'occhio li nota. Il dettaglio dentro la sagoma va nella normal map.
- **Niente triangoli lunghi e sottili, né minuscoli.** La GPU colora i pixel a gruppi di quattro: un triangolo di pochi pixel, o lungo e sottile, la fa lavorare a vuoto. Da lontano lo risolve il LOD, da vicino una rete ordinata.
- **Il nome e la scala.** Esporta con **Apply Scalings: FBX Units Scale**, e chiama le versioni semplificate col nome del modello più `_LOD1` e `_LOD2`.
> lavoro: 3 2

## Trucchi del mestiere · in Unity
- **Il collider semplice.** Per il teletrasporto e per afferrare basta un **Box Collider** o un **Capsule Collider**. Un **Mesh Collider** sul modello pieno fa lavorare la fisica su tutti i suoi triangoli: se serve la forma, dagli la versione LOD 2.
- **Il far plane.** La camera non disegna niente oltre il **Far** dei suoi **Clipping Planes**: tienilo lungo quanto serve alla scena, non di più.
- **Il passaggio dolce.** Se il salto fra due livelli si vede, nel LOD Group scegli **Fade Mode: Cross Fade** e spunta **Animate Cross-fading**: per un attimo Unity disegna i due livelli insieme e li sfuma.
- **La cartolina.** Per le cose lontanissime, come le montagne o gli alberi in fondo, basta un'immagine: un quad con la foto dell'oggetto, che si chiama **billboard**, o lo skybox.
- **Il terreno.** Il **Terrain** di Unity ha il **Pixel Error**: più è alto, meno triangoli usa da lontano.
- **Mesh LOD o static batching.** Con lo static batching, che vediamo nella lezione 7, Unity disegna sempre il livello pieno del Mesh LOD: scegli l'uno o l'altro.
- **In VR gli oggetti si avvicinano.** Quello che il giocatore prende in mano arriva a venti centimetri dagli occhi: lì servono dettaglio e normal map. Un soffitto a tre metri ne ha bisogno molto meno.
- **Le facce da due parti.** Un materiale con **Render Face: Both** disegna anche il retro di ogni faccia: usalo solo per foglie e stoffe sottili.
> lavoro: 3 2

# Da soli · Verifica

## Le tre sale
Tre sale, tre clienti, tre budget. Per ogni sala il giro è lo stesso del salone:
1. il Display della sala nella Game view, e **Tris** scritto nel file delle misure;
2. la foto del prima;
3. la cura;
4. di nuovo **Tris** nel file, e la foto del dopo.
Le regole del curatore valgono sempre. Non spegnere, non cancellare e non spostare niente. Un museo con le opere spente non è un museo ottimizzato. E la foto del dopo deve sembrare quella del prima.
Il giardino e le curiosità sono nella scena del museo, in questa lista. Il deposito ha una scena sua e una lista sua, nel capitolo dopo: lo fai per ultimo.
Se hai fatto la cura e un compito resta **Da fare**, nella sala qualcosa è stato spostato o spento: annulla con **Ctrl+Z**, o chiamami. Alla fine di ogni sala salva con **Ctrl+S**.
Prima di tutto riapri il museo: la lista controlla la scena aperta. Unity chiede se salvare il laboratorio: rispondi **Save**.
> azione: Apri il museo | apri-scena | Lezione 06 - Geometria | Museo
> verifica: scena-aperta Museo
> soluzione: Premi **Apri il museo** e, alla domanda di Unity, **Save**.

## Il giardino · il prima
Il giardino è sul **Display 3**. Leggi **Tris** e scrivilo nel file. Poi la foto.
> azione: Foto del giardino, prima | foto-camera | Camera Giardino | Consegne/L06/giardino-prima.jpg
> verifica: file-esiste Consegne/L06/giardino-prima.jpg
> soluzione: Nella Game view scegli **Display 3**, leggi **Tris** e scrivilo nel file delle misure. Poi premi il pulsante della foto del prima.

## Il giardino · la cura
Trenta statue all'aperto, dalla più vicina alla più lontana. Il cliente vuole il giardino nel budget del cartello.
> verifica: lod-su Giardino | Statua
> lavoro: 6 3
> soluzione: Le statue sono copie di due prefab, `Statua gotica` e `Cavallo`, nella cartella `Prefab` del museo.
> soluzione: Apri `Statua gotica` con un doppio clic, seleziona `Statua gotica` nella Hierarchy e scegli **RUFA ▸ Strumenti ▸ LOD Group dalle varianti selezionate**. Esci con la freccia in alto nella Hierarchy.
> soluzione: Fai lo stesso con `Cavallo`. Poi, sul **Display 3**, controlla **Tris**. Se le statue vicine cambiano aspetto in modo visibile, sposta più in basso la soglia del LOD 1 nei due prefab.

## Il giardino · il dopo
Rileggi **Tris** sul **Display 3**, scrivilo nel file e fai la foto del dopo.
> azione: Foto del giardino, dopo | foto-camera | Camera Giardino | Consegne/L06/giardino-dopo.jpg
> verifica: file-esiste Consegne/L06/giardino-dopo.jpg
> soluzione: Sul **Display 3** leggi **Tris** e scrivilo nel file. Poi premi il pulsante della foto del dopo.

## Le curiosità · il prima
La sala delle curiosità è sul **Display 5**. Leggi **Tris** e scrivilo nel file. Poi la foto.
> azione: Foto delle curiosità, prima | foto-camera | Camera Curiosità | Consegne/L06/curiosita-prima.jpg
> verifica: file-esiste Consegne/L06/curiosita-prima.jpg
> soluzione: Nella Game view scegli **Display 5**, leggi **Tris** e scrivilo nel file delle misure. Poi premi il pulsante della foto del prima.

## Le curiosità · la cura
Scaffali pieni di oggetti piccoli: orologi da tasca, bussole, occhiali. Da lontano sono pochi pixel, e ognuno ha migliaia di triangoli. Attento: il curatore conosce bene i suoi scaffali.
> verifica: lod-su Curiosità | Oggetto
> lavoro: 7 4
> soluzione: Gli oggetti sono copie di tre prefab: `Orologio da tasca`, `Bussola`, `Occhiali`. Su ognuno usa **RUFA ▸ Strumenti ▸ LOD Group dalle varianti selezionate**, come per le statue.
> soluzione: Guarda la Game view sul **Display 5**: gli scaffali in fondo si sono svuotati. Lo strumento mette la soglia di **Culled** al 3%, e oggetti così piccoli scendono sotto il 3% già a pochi metri.
> soluzione: Nei tre prefab trascina il confine fra **LOD 2** e **Culled** verso destra, un poco alla volta, finché gli scaffali in fondo non sono di nuovo pieni. Controlla che **Tris** stia ancora nel budget.

## Le curiosità · il dopo
Rileggi **Tris** sul **Display 5**, scrivilo nel file e fai la foto del dopo.
> azione: Foto delle curiosità, dopo | foto-camera | Camera Curiosità | Consegne/L06/curiosita-dopo.jpg
> verifica: file-esiste Consegne/L06/curiosita-dopo.jpg
> soluzione: Sul **Display 5** leggi **Tris** e scrivilo nel file. Poi premi il pulsante della foto del dopo.

# Da soli · Verifica · Il deposito

## Il deposito · la sala
Il deposito ha una scena sua. Premi il pulsante: Unity ti chiede se salvare il museo, e tu rispondi **Save**, così le cure del giardino e delle curiosità restano.
> azione: Apri il deposito | apri-scena | Lezione 06 - Geometria | Deposito
> verifica: scena-aperta Deposito
> soluzione: Premi **Apri il deposito** e, alla domanda di Unity, **Save**.

## Il deposito · il prima
Il deposito è sul **Display 4**. Leggi **Tris** e scrivilo nel file. Poi la foto.
> azione: Foto del deposito, prima | foto-camera | Camera Deposito | Consegne/L06/deposito-prima.jpg
> verifica: file-esiste Consegne/L06/deposito-prima.jpg
> soluzione: Nella Game view scegli **Display 4**, leggi **Tris** e scrivilo nel file delle misure. Poi premi il pulsante della foto del prima.

## Il deposito · la cura
Un'anticamera con qualche cassa e una porta chiusa. Il cliente dice che si vede poco, eppure costa tantissimo. Trova perché e cura la sala.
> verifica: occlusion-su Deposito | Muro del magazzino
> lavoro: 5 3
> soluzione: Dietro il `Muro del magazzino` c'è il magazzino, pieno di casse e botti che la camera non vede e il visore disegna. Lo vedi nella Scene view, o in Wireframe.
> soluzione: Seleziona `Deposito` nella Hierarchy e scegli **RUFA ▸ Strumenti ▸ Statici: imposta flag (selezione)**.
> soluzione: Poi **RUFA ▸ Strumenti ▸ Occlusion culling: bake**. Il bake rifà anche quello del salone, che resta com'era.

## Il deposito · il dopo
Rileggi **Tris** sul **Display 4**, scrivilo nel file e fai la foto del dopo.
> azione: Foto del deposito, dopo | foto-camera | Camera Deposito | Consegne/L06/deposito-dopo.jpg
> verifica: file-esiste Consegne/L06/deposito-dopo.jpg
> soluzione: Sul **Display 4** leggi **Tris** e scrivilo nel file. Poi premi il pulsante della foto del dopo.

## Il curatore
Come nel salone, ma per le tue tre sale: il compagno fa il curatore sulle tue foto, e tu sulle sue. Per ogni sala gli mostri le due foto a tutto schermo, nell'ordine che decidi tu, senza fargli vedere i nomi dei file. Se ti indica una differenza che c'è davvero, torna nella sala e regola: le soglie dei LOD, la soglia di Culled. Scrivi nel file delle misure cosa ha notato e cosa hai cambiato.
> lavoro: 4 3

# Da soli · Il tuo progetto

## La tua scena
Ora il tuo progetto. Premi il pulsante: si apre la scena del tuo blockout.
> azione: Apri la tua scena | crea-progetto
> verifica: scena-aperta Progetto

## Arreda
Un blockout fatto di cubi costa poco. Il costo arriva con i modelli veri. Arreda il tuo blockout: trascina nella scena i mobili di `Assets/Demo/Models` e le opere del museo, già coi loro materiali, da `Assets/Samples/RUFA Core/Lezione 06 - Geometria/Prefab`, oppure i tuoi modelli.
Chi modella può portare un suo modello e preparare in Blender le varianti `_LOD1` e `_LOD2` col Decimate, esportate nella stessa cartella del modello.
> lavoro: 20 12

## La camera
Ti serve una camera fissa nel punto che conta di più: dove il giocatore guarda di più, o dove la scena è più piena.
1. Nella Scene view inquadra quel punto, all'altezza degli occhi.
2. Dal menu in alto scegli **GameObject ▸ Camera**. Rinomina la camera `Camera foto`.
3. Con la camera selezionata scegli **GameObject ▸ Align With View**: la camera prende l'inquadratura della Scene view.
4. Nell'Inspector della camera metti **Target Display** su **Display 2**.
> verifica: scena-componente Camera foto | Camera
> lavoro: 4 2

## Il prima
1. Nella Game view scegli **Display 2** e leggi **Tris**.
2. Scrivilo nel file delle misure, nella parte del progetto, con il punto dove hai messo la camera.
3. Fai la foto.
> azione: Foto del progetto, prima | foto-camera | Camera foto | Consegne/L06/progetto-prima.jpg
> verifica: file-esiste Consegne/L06/progetto-prima.jpg

## La cura
Usa le tecniche di oggi dove servono:
- superfici piatte con troppi triangoli: meno suddivisioni;
- modelli che si vedono anche da lontano: il LOD Group;
- stanze separate da muri: statici e occlusion culling.
Non tutto serve dappertutto. Scrivi nel file cosa hai fatto e perché.
> lavoro: 25 15

## Il dopo
1. Rileggi **Tris** sul **Display 2** e scrivilo nel file.
2. Fai la foto del dopo.
3. Salva la scena con **Ctrl+S**.
> azione: Foto del progetto, dopo | foto-camera | Camera foto | Consegne/L06/progetto-dopo.jpg
> verifica: file-esiste Consegne/L06/progetto-dopo.jpg

## Le misure
Controlla che nel file delle misure non resti nessun «da scrivere».
> verifica: file-compilato Consegne/L06/misure.md | «da scrivere»
> lavoro: 5 3

## A fine lezione
Salva il lavoro. Nella cartella della tua copia lancia:
`git salva "Lezione 06"`
