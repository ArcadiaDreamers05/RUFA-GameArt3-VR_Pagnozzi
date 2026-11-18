# Lezione 7 — Le draw call nel mercato coperto: la guida

Questa è la guida da tenere aperta mentre lavori. Ci ho messo le stesse procedure del tutorial, tutte in un posto solo e in ordine, il laboratorio di Blender passo per passo e i trucchi del mestiere. E ci ho messo tutte le regole che Meta e Unity danno per la VR su questi temi. Ogni regola ha il suo link, e in fondo trovi l'elenco completo delle fonti.

## 1. Il cliente e il budget
Oggi lavori per un altro cliente. Il mercato coperto della città vuole una visita in realtà virtuale, sul Quest 3S. I banchi sono pieni: frutta, spezie, dolci, un bar, cassette regalo. Il problema non sono i triangoli: quelli li hai curati nella lezione 6. Il problema è il numero di cose da disegnare.

**La draw call.** Per disegnare un oggetto la CPU, il processore, deve dire alla GPU, la scheda grafica: «disegna questa mesh con questo materiale». Quest'ordine si chiama **draw call**. Meta la definisce così: una draw call avviene quando un materiale e una mesh vengono mandati alla GPU per essere disegnati. Di partenza ogni oggetto costa almeno una draw call: Unity scrive che ogni mesh ha bisogno della sua. Un oggetto con tre materiali ne costa tre, perché Unity disegna ogni materiale per conto suo.

**Il SetPass.** Prima dell'ordine la CPU prepara la GPU: lo shader giusto, le texture, le impostazioni del materiale. Unity scrive che questa preparazione è il passo che pesa di più sulla CPU ([Unity, le draw call](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/optimizing-draw-calls)). Il cambio di shader si chiama **SetPass**. Se due oggetti di fila usano lo stesso shader con le stesse opzioni, il secondo non ha bisogno di un SetPass.

Meta l'ha misurato su un visore, un Quest 1 con una versione vecchia di Unity, e scrive che i risultati valgono per qualsiasi app ([Meta, Draw Call Cost Analysis](https://developers.meta.com/vr/documentation/unity/po-draw-call-analysis/)):
- il costo di una draw call dipende da quella prima: se cambiano il materiale, la mesh o la texture, la draw call costa di più;
- con lo stesso shader, cambiare materiale fra una draw call e l'altra la allunga del 64%;
- cambiare shader la allunga del 175%;
- ridisegnare lo stesso oggetto costa circa un quarto di quanto costa disegnarne uno diverso.

Quindi non conta solo quante draw call fai. Conta anche quanto cambia fra una e l'altra.

**In VR costa di più.** Meta lo spiega nella stessa pagina. Un gioco in VR ha più draw call di un gioco 2D, gira a più frame al secondo, e ogni oggetto si disegna per due occhi. Il nostro progetto chiede al visore 90 frame al secondo: la CPU del visore, che è piccola, ha 11 millisecondi per preparare tutto il frame. Il rendering single pass, che vediamo nella lezione 10, disegna i due occhi in una volta e toglie un po' di questo peso alla CPU.

**Il budget di Meta.** Meta dice quante draw call per frame può reggere un Quest 3 o un 3S, secondo quanto lavora l'app ([Meta, Testing and performance analysis](https://developers.meta.com/vr/documentation/unity/unity-perf/)):
- da 200 a 300 per le app con tanta simulazione: tanti giocatori o personaggi animati, la rete, la chat vocale. Meta cita gli sparatutto multigiocatore e le app social affollate;
- da 400 a 600 per la maggior parte delle app: mondi medi, con pochi giocatori o personaggi, come uno sparatutto per un giocatore solo;
- da 700 a 1000 per le app leggere, quelle che fra una draw call e l'altra cambiano poco lo stato della GPU, cioè shader, texture e mesh. Meta cita le escape room, i puzzle e i giochi cooperativi.

Guarda bene la terza riga. Il budget più alto vale solo per chi cambia poco fra un disegno e l'altro: è proprio quello che curi oggi. Meta avverte che sono numeri indicativi. Contano anche il lavoro della CPU per animazioni, personaggi e rete, e le impostazioni grafiche. Per i cambi di stato dà tre consigli: condividi le mesh e usa l'instancing dove puoi; usa i texture array, se puoi; usa pochi shader unificati, che non creano varianti. Il budget è il totale del frame, per tutta la scena.

Meta, nelle regole per le app autonome, lo dice ancora più semplice: sii prudente con le prestazioni fin dall'inizio, e tieni basse le draw call ([Meta, Android Development](https://developers.meta.com/vr/documentation/unity/unity-mobile-performance-intro/)).

Se il tuo progetto è una escape room, il tuo budget sta nella terza riga. Se è un walking simulator con poca simulazione, anche. Il budget di solito non lo decidi tu: lo dà il cliente o lo studio. Il nostro cliente l'ha diviso fra i banchi e l'ha scritto su una lavagna in fondo a ogni bottega: la frutta 160 Batches, le spezie 110, la vetrina dei dolci al massimo 3 strati di vetro, il bar 60, le cassette miste 80.

## 2. Il mercato
**I modelli nuovi.** La prima pagina del tutorial controlla che nel progetto ci siano le merci del mercato, in `Assets/Demo/Models/Mercato`. Se resta **Da fare**, chiudi Unity, lancia `git aggiorna` un'altra volta nella cartella della tua copia e riapri il progetto. Il primo `git aggiorna` porta il comando nuovo, il secondo porta i file. Unity Hub deve essere aperto, perché il progetto riscarica i suoi package.

**Il rig del PC.** Oggi il visore resta spento. Dal menu **RUFA ▸ Play mode**, se accanto a **Simulatore VR** c'è la spunta, clicca la voce: la spunta sparisce. In Play clicca nella Game view per guardare col mouse e cammina con **W A S D**. **Esc** ti ridà il mouse.

**Apri il mercato.** Il pulsante **Apri il mercato** del tutorial copia il mercato in `Assets/Samples/RUFA Core/Lezione 07 - Draw call` e apre la scena `Mercato`. La copia è tua: se la rovini, cancella quella cartella e premi di nuovo il pulsante.

**Le cinque camere.** Lungo la navata ci sono cinque botteghe. Ognuna ha una camera fissa sull'apertura, che guarda dentro e vede solo la sua bottega.
1. Clicca la linguetta **Game**.
2. In alto a sinistra nella Game view apri il menu **Display 1** e scegli il display del banco: **Display 2** la frutta, **Display 3** le spezie, **Display 4** la pasticceria, **Display 5** il bar, **Display 6** le cassette. Il **Display 1** è il tuo rig, in Play.
3. Nel menu delle proporzioni, quello con scritto **Free Aspect**, scegli **16:9 Aspect**. Così l'inquadratura è la stessa su tutti i PC.
4. Premi **Stats** in alto a destra: compare il riquadro coi numeri del frame.

Ogni display disegna solo le sue camere, e Stats conta solo quello che disegna il display scelto.

**La tua insegna.** Nella bottega della frutta, in alto sulla parete di fondo, c'è un'insegna bianca. È per te: se hai un tuo disegno o una tua grafica, mettila lì. Trascina il file PNG o JPG nella cartella `Assets/Progetto`. Poi seleziona `Tela`, dentro `Banco della frutta`, apri il materiale `Insegna` in fondo all'Inspector e trascina l'immagine sul quadratino accanto a **Base Map**. L'insegna sta fuori dalle merci, quindi non cambia le verifiche.

**Il file delle misure.** Il pulsante del tutorial crea `Consegne/L07/misure.md` e lo apre nel blocco note. Tienilo aperto: ci scrivi i numeri di ogni banco, prima e dopo.

## 3. Come si misura
**Cosa conta Stats.** Oggi guardi tre numeri. Te li spiego con le parole del manuale di Unity ([Unity, la finestra Statistics](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/profiling/rendering-statistics)).
- **Batches**: le draw call del frame, contate dopo che Unity ne ha messe insieme quante poteva. Un gruppo di oggetti disegnati insieme conta uno. Il numero comprende i gruppi dello static batching, del dynamic batching e dell'instancing, che trovi più avanti.
- **Saved by batching**: quante draw call Unity ha messo insieme nei gruppi. Unity aggiunge un consiglio: condividi i materiali fra più oggetti che puoi, perché un gruppo raccoglie le draw call con lo stesso stato, e ogni cambio di materiale ne apre uno nuovo.
- **SetPass calls**: quante volte Unity cambia la passata dello shader con cui disegna. Ogni cambio porta sulla GPU uno shader nuovo, e costa alla CPU.

Unity stesso, per capire se una scena manda troppe draw call, indica questi tre strumenti: le **SetPass calls** in Stats, il modulo **Rendering** del Profiler e il Frame Debugger ([Unity, scegliere il metodo](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/optimizing-draw-calls-choose-method)). Meta consiglia Stats per controllare il batching nell'editor: ti dice quante draw call partono e quante ne risparmi ([Meta, Testing and performance analysis](https://developers.meta.com/vr/documentation/unity/unity-perf/)).

**Si misura in Play.** Fuori dal Play Unity disegna ogni oggetto per conto suo. Una delle cure di oggi, lo **static batching**, Unity la fa quando parte il Play, o quando costruisce l'app per il visore. Per questo oggi misuri sempre in Play. La regola è questa: misuri in Play, curi fuori dal Play. Quello che cambi in Play sparisce quando esci, come hai visto nella lezione 5.
1. Premi **Play**.
2. Nella Game view scegli il display del banco. Se il menu non risponde, premi **Esc** per riavere il mouse.
3. Leggi **Batches**, **Saved by batching** e **SetPass calls** e scrivili nel file delle misure, alla riga del banco.
4. Confronta **Batches** col budget sulla lavagna.
5. Premi di nuovo **Play** per uscire.

**Sul PC i numeri sono più alti.** Come per i triangoli, Stats conta ogni passata. Quando l'editor è impostato per il PC, il nostro progetto disegna gli oggetti opachi una volta in più, per l'occlusione ambientale (SSAO), l'ombra morbida negli angoli della versione PC. Per questo sul PC vedi circa il doppio delle draw call. I budget delle lavagne sono pensati per l'editor impostato per il PC: sul visore hai più margine. Con l'editor su Android, per esempio dopo una build per il visore, quella passata non c'è e i numeri sono più bassi.

Sul visore l'SSAO non c'è, ed è giusto così. Unity dice di spegnerlo sui visori autonomi, perché sulle loro GPU richiede passate che costano molto ([Unity, visori autonomi](https://docs.unity.com/en-us/engine/6000.3/manual/xr/graphics/untethered-device-optimization)). Meta lo mette fra gli effetti da evitare, e chiede di tenere al minimo le passate di disegno ([Meta, Best practices for VR](https://developers.meta.com/vr/documentation/unity/unity-best-practices-intro/) e [Meta, Android Development](https://developers.meta.com/vr/documentation/unity/unity-mobile-performance-intro/)). Ogni passata in più ridisegna gli oggetti.

**Il Frame Debugger.** Stats ti dice quante draw call ci sono. Il **Frame Debugger** ti fa vedere quali sono, una per una ([Unity, Frame Debugger](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/profile-rendering/frame-debugger/debug)).
1. Premi **Play** e scegli il display del banco.
2. Dal menu in alto apri **Window ▸ Analysis ▸ Frame Debugger** e premi **Enable**. Il gioco si ferma su un frame.
3. A sinistra c'è l'elenco di tutto quello che Unity ha disegnato. Apri le righe col triangolino finché trovi le **SRP Batch**: sono gruppi di draw call che Unity ha preparato insieme.
4. Clicca un gruppo. A destra, **Batch cause** dice perché quel gruppo si è interrotto e non ha preso anche l'oggetto dopo. Al banco della frutta leggerai spesso che lo shader o le sue opzioni cambiano.
5. Trascina il cursore in alto nel Frame Debugger: la Game view disegna il frame fino a quel punto, un ordine alla volta.
6. Premi **Disable** ed esci dal Play.

Unity dà una regola per leggerlo: se tante SRP Batch contengono poche draw call, di solito il progetto usa troppe varianti degli shader ([Unity, controllare l'SRP Batcher](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/in-urp/reduce-draw-calls-urp/srpbatcher/srpbatcher-profile)). Nei dettagli di un disegno, alla voce **Meshes**, una **Combined Mesh** vuol dire che quel disegno viene dallo static batching ([Unity, preparare il batching](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/draw-call-batching/set-up)). Meta consiglia il Frame Debugger anche quando non c'è niente da correggere: fa capire come Unity costruisce la scena.

**Sul visore.** Stats sul PC conta le passate del PC. I numeri veri del Quest li leggi col Profiler collegato al visore, nel modulo **Rendering**, che mostra **Batches**, **SetPass Calls** e **Draw Calls** ([Unity, il modulo Rendering](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/profile-rendering/profiler-rendering)), e con OVR Metrics, come nella lezione 5. Unity lo ripete a ogni ottimizzazione. Prima di cambiare qualcosa, capisci dov'è il problema: se no rischi di perdere tempo, o di peggiorare. E misura sempre sulla piattaforma di destinazione ([Unity, ridurre il lavoro di CPU e GPU](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/optimizing-graphics-performance) e [Unity, ottimizzare gli shader](https://docs.unity.com/en-us/engine/6000.3/manual/materials-and-shaders/shaders/sl-shader-performance)).

## 4. Uno shader, un materiale
**L'SRP Batcher.** URP ha un aiutante, l'**SRP Batcher**. Non riduce le draw call: riduce il lavoro fra una e l'altra. Tiene i dati dei materiali nella memoria della GPU, e prepara insieme gli oggetti che usano la stessa variante dello stesso shader, anche se ognuno ha il suo materiale. Ogni gruppo preparato così è una SRP Batch. Le regole:
- [Unity](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/in-urp/reduce-draw-calls-urp/srpbatcher/srpbatcher): per avere gruppi lunghi usa meno varianti possibile dello shader. I materiali con lo stesso shader invece possono essere quanti vuoi.
- [Unity, nell'asset di URP](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/urp-reference/universalrp-asset): l'SRP Batcher serve quando hai tanti materiali diversi con lo stesso shader, e accelera soprattutto il lavoro della CPU.
- [Unity, scegliere il metodo](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/optimizing-draw-calls-choose-method): in URP va acceso.
- [Meta](https://developers.meta.com/vr/documentation/unity/unity-perf/): usa un insieme minimo di shader unificati, che non generano varianti.
- [Unity, accendere l'SRP Batcher](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/in-urp/reduce-draw-calls-urp/srpbatcher/srpbatcher-enable): se gli shader di un progetto non sono adatti all'SRP Batcher, su un dispositivo debole può andare meglio spento. Gli shader di URP sono tutti adatti, tranne quelli delle particelle: nel nostro progetto resta acceso.

Un oggetto entra nell'SRP Batcher se ha una mesh, normale o di un personaggio con lo scheletro, se non è una particella, se non usa un `MaterialPropertyBlock` e se il suo shader è compatibile ([Unity, la compatibilità](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/in-urp/reduce-draw-calls-urp/srpbatcher/srpbatcher-materials)). Se scrivi shader tuoi, la compatibilità la leggi nell'Inspector dello shader.

**L'esperimento.** Spegni l'SRP Batcher per un momento, per vedere quanto lavora.
1. Nella finestra **Project** apri `Assets/Settings` e seleziona `PC_RPAsset`: sono le impostazioni di URP che l'editor usa per il PC.
2. Nell'Inspector, nel riquadro **Rendering**, clicca i tre puntini a destra del titolo e scegli **Show All Advanced Properties**. Compare la casella **SRP Batcher**.
3. Togli la spunta a **SRP Batcher**.
4. Premi **Play**, scegli **Display 2** e leggi **SetPass calls**: è esploso, centinaia, più dei frutti. Scrivilo nel file, alla riga dell'esperimento.
5. Esci dal Play e rimetti la spunta. Se lo lasci spento, peggiori anche il tuo progetto: il tutorial controlla che sia di nuovo acceso.

**Le varianti.** Uno shader è una famiglia. Ogni opzione che accendi nel materiale crea una **variante**, cioè una versione diversa dello stesso shader: una normal map, i riflessi speculari, l'**Enable GPU Instancing**. Due materiali con lo stesso shader, ma con la normal map solo nel primo, sono due varianti, e l'SRP Batcher li prepara separati. Per questo la cura è un solo shader, con le stesse opzioni per tutto il banco.

**Gli shader di URP**, dal più pesante al più leggero ([Unity, scegliere lo shader](https://docs.unity.com/en-us/engine/6000.3/manual/materials-and-shaders/built-in/shaders-in-universalrp/choose) e [Unity, i modelli di luce](https://docs.unity.com/en-us/engine/6000.3/manual/materials-and-shaders/built-in/shaders-in-universalrp/shading-model)):
- **Complex Lit**: il Lit con in più effetti avanzati, come lo strato trasparente delle vernici lucide, il **Clear Coat**. Unity dice di evitarlo, perché fa calcoli di luce complessi; se proprio lo usi, spegni **Clear Coat** ([Unity, configurare URP per le prestazioni](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/introduction/configure-for-better-performance)).
- **Lit**: lo shader realistico, fisicamente corretto, il più completo che usi di solito. Unity scrive che usa il modello di luce più pesante di URP, e che questo modello non è adatto all'hardware mobile di fascia bassa: lì consiglia gli shader col modello semplice.
- **Simple Lit**: una luce più semplice e un riflesso meno fisico. Unity lo consiglia quando le prestazioni contano più del fotorealismo, per gli stili non realistici e per le piattaforme meno potenti. È il caso del visore.
- **Baked Lit**: solo la luce cotta nelle lightmap e nelle light probe, senza luci in tempo reale. Lo vediamo nella lezione 8.
- **Unlit**: nessuna luce, solo il colore. Il più leggero di tutti, ottimo per l'hardware più debole.

Unity dà anche una regola pratica per le piattaforme mobili meno potenti: Baked Lit per gli oggetti fermi e Simple Lit per quelli che si muovono ([Unity, configurare URP per le prestazioni](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/introduction/configure-for-better-performance)). Meta, nelle regole scritte per la pipeline vecchia di Unity, consiglia le versioni Mobile o Unlit degli shader e di evitare lo Standard, lo shader realistico di allora ([Meta, Android Development](https://developers.meta.com/vr/documentation/unity/unity-mobile-performance-intro/)). E aggiunge un'idea da artista: prova anche senza texture, perché con gli shader giusti un gioco può essere bellissimo lo stesso ([Meta, Draw Call Cost Analysis](https://developers.meta.com/vr/documentation/unity/po-draw-call-analysis/)).

Al banco della frutta ogni tipo di frutto è arrivato con uno shader diverso: chi su Lit, chi su Complex Lit, chi su Simple Lit con opzioni diverse.

**Le copie.** Al banco della frutta ogni mela ha la sua copia del materiale: `Mela`, `Mela.001`, `Mela.002`. Sono i nomi che dà Blender quando importi lo stesso modello più volte: il fruttivendolo ha importato ogni cassetta per conto suo. Con l'SRP Batcher acceso le copie della stessa variante non costano SetPass. Però bloccano lo static batching, che mette insieme solo gli oggetti con lo stesso materiale. Quindi le copie si fondono: un materiale per ogni tipo di frutto. Le regole:
- [Unity](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/optimizing-draw-calls-choose-method): qualunque metodo usi, usa gli stessi materiali per oggetti diversi, più che puoi.
- [Meta](https://developers.meta.com/vr/documentation/unity/unity-mobile-performance-intro/): aiuta il batching. Condividi i materiali e usa un atlas quando puoi.
- [Meta](https://developers.meta.com/vr/documentation/unity/po-draw-call-analysis/): fra una draw call e l'altra evita di cambiare shader, e poi materiale. Per ridurre il costo dei cambi di materiale, ordina le draw call per materiale. Evita anche di cambiare mesh, se puoi: pesa meno del materiale, ma pesa.
- [Meta](https://developers.meta.com/vr/documentation/unity/po-draw-call-analysis/): se fra due oggetti cambia solo il colore, usa lo stesso materiale e cambia il colore della mesh, cioè il colore dei suoi vertici. Gli shader Lit e Simple Lit di URP però non leggono il colore dei vertici: serve uno shader fatto apposta. Con gli shader di URP la strada più semplice è la palette, nei trucchi del mestiere.
- [Unity](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/optimizing-draw-calls-choose-method): se vuoi la stessa mesh con proprietà diverse, per esempio colori diversi, usa le **Material Variant** invece di tanti materiali separati. Una Material Variant è un materiale figlio di un altro: ne eredita tutto, tranne quello che cambi.

**La cura alla frutta, in breve.** Prima le copie:
1. Nella Hierarchy, nella casella di ricerca in alto, scrivi `Mela`. Restano solo le mele.
2. Clicca la prima, poi **Shift**+clic sull'ultima: sono tutte selezionate.
3. Nella finestra **Project** apri `Assets/Samples/RUFA Core/Lezione 07 - Draw call/Materiali/Frutta`. Trascina il materiale `Mela`, quello senza numero, sull'Inspector, sopra il campo del materiale nel componente **Mesh Renderer**, alla voce **Materials**.
4. Fai lo stesso per `Pere`, `Limone`, `Lime`, `Kiwi`, `Avocado`, `Melograno`, `Litchi`, `Cesto` e `Fruttiera`, ognuno col suo materiale senza numero.
5. Svuota la ricerca con la crocetta e salva con **Ctrl+S**.

Poi lo shader:
1. Nella cartella `Materiali/Frutta` clicca il primo materiale e premi **Ctrl+A**: sono tutti selezionati.
2. In cima all'Inspector, nel menu **Shader**, scegli **Universal Render Pipeline ▸ Simple Lit**.
3. In fondo, sotto **Advanced Options**, metti la spunta a **Specular Highlights**. Se la casella mostra un trattino, i materiali sono diversi: cliccala finché vedi la spunta. Dopo il cambio di shader il trattino c'è quasi sempre: Lit e Simple Lit leggono quell'opzione al contrario.
4. In fondo, sotto **Advanced Options**, togli la spunta a **Enable GPU Instancing**. Anche qui un trattino vuol dire materiali diversi.
5. Guarda la frutta nella Game view: è uguale a prima? Se un frutto ti sembra troppo lucido o troppo opaco, selezionane il materiale e regola **Smoothness**.

Poi rileggi i numeri in Play. **SetPass calls** è sceso. **Batches** invece è quasi uguale: l'SRP Batcher prepara meglio le draw call, ma non ne toglie nessuna. Per toglierle serve la sezione dopo.

**Negli script.** Per cambiare il materiale da uno script usa `sharedMaterial`, non `material`. Unity avverte: `material` crea una copia del materiale per quell'oggetto, e la copia impedisce a Unity di mettere insieme le sue draw call. E in URP niente `MaterialPropertyBlock`, il modo di cambiare una proprietà a un oggetto solo senza toccare il materiale: toglie l'oggetto dall'SRP Batcher. Unity consiglia materiali diversi per proprietà diverse ([Unity, le proprietà delle mesh unite](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/draw-call-batching/properties)).

## 5. Fermi e uniti
**Lo static batching.** La frutta del banco non si muove mai. Unity può allora unire le sue mesh in una mesh grande, una volta sola, con le coordinate del mondo, e disegnare tanti frutti con poche draw call. Si chiama **static batching** ([Unity, il batching](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/draw-call-batching/draw-call-batching)). Servono due cose ([Unity, lo static batching nella build](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/draw-call-batching/enable)):
- il flag **Batching Static** sugli oggetti, nella casella **Static** dell'Inspector;
- l'opzione **Static Batching** nelle impostazioni del Player, in **Edit ▸ Project Settings ▸ Player**, sezione **Other Settings**. Nel nostro progetto è già accesa.

Unity unisce solo oggetti con lo stesso materiale. Più precisamente, con la stessa variante dello shader, le stesse texture, la stessa lightmap e lo stesso layer. Non unisce le mesh molto piccole, quelle con la dissolvenza fra i livelli di LOD, e mesh con dati diversi: una mesh con le UV e una senza restano separate ([Unity, preparare il batching](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/draw-call-batching/set-up)). Unisce solo i **Mesh Renderer**, non le mesh dei personaggi con lo scheletro. Se usi lo static batching nella build, Unity non spende CPU a unire le mesh mentre il gioco gira: le unisce prima.

Ha dei costi, e Unity li scrive tutti:
- la memoria cresce, perché Unity tiene una copia delle mesh unite, anche se sono uguali. In un bosco fitto, segnare statici tutti gli alberi pesa molto. Se la memoria non basta, unisci le mesh a mano;
- un gruppo arriva al massimo a 64.000 vertici, poi ne comincia un altro;
- se un pezzo esce dall'inquadratura, Unity lo scarta e il gruppo si può spezzare in due draw call;
- in gioco puoi spostare solo il gruppo intero, non i suoi pezzi ([Unity, il batching mentre il gioco gira](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/draw-call-batching/static-batching-enable)). Meta lo dice uguale: lo static batching è per gli oggetti che non si spostano, non girano e non cambiano scala, e va segnato a mano su ogni oggetto ([Meta, Android Development](https://developers.meta.com/vr/documentation/unity/unity-mobile-performance-intro/)).

**La tabella di Unity e il Quest.** Nella pagina in cui Unity confronta i metodi, per URP trovi scritto di spegnere il batching e di accendere il **GPU Resident Drawer**, perché lo static batching non va d'accordo con lui ([Unity, scegliere il metodo](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/optimizing-draw-calls-choose-method)). Vale per chi usa il GPU Resident Drawer. Per i visori autonomi come il Quest, il manuale più recente di Unity dice di evitarlo, come leggi qui sotto. Per questo nel nostro progetto la cura degli oggetti fermi resta lo static batching, che è anche quella di Meta.

**La frutta ferma.**
1. Nella Hierarchy apri `Banco della frutta` e seleziona `Frutta`.
2. In alto a destra nell'Inspector c'è la casella **Static** con una freccia. Apri la freccia e scegli **Batching Static**.
3. Unity chiede se vale anche per i figli: rispondi **Yes, change children**.
4. Salva con **Ctrl+S**.
5. Premi **Play**, scegli **Display 2** e rileggi i numeri. Ora **Batches** è crollato e **Saved by batching** è salito. Scrivili nel file ed esci dal Play.

**L'atlas.** Lo static batching mette insieme oggetti con lo stesso materiale. Ma due materiali uguali in tutto tranne la texture restano due. La soluzione è l'**atlas**: le texture dei due materiali si mettono in un'immagine sola, più grande, e i due materiali diventano uno.
- [Unity](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/draw-call-batching/set-up): se hai due materiali identici tranne le texture, metti le texture in una texture sola, più grande. E condividi i materiali fra più oggetti possibile.
- [Meta](https://developers.meta.com/vr/documentation/unity/po-draw-call-analysis/): per ridurre il tempo delle draw call ordina gli oggetti, fai l'atlas delle texture, e prova anche senza texture.
- [Meta](https://developers.meta.com/vr/documentation/unity/unity-perf/) cita anche i texture array: una pila di texture tutte della stessa grandezza, da cui lo shader sceglie quella giusta. Servono shader scritti apposta: oggi ci basta l'atlas.

L'atlas lo fai nel laboratorio, nella sezione 7.

**Unire a mano.** Puoi anche unire le mesh in un oggetto solo, nel programma 3D o in Unity. Fa una draw call sola, ma c'è un prezzo: Unity non può più scartare i pezzi uno per uno. Se ne vedi un pezzo, Unity disegna tutto l'oggetto, anche la parte alle tue spalle ([Unity, unire le mesh a mano](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/draw-call-batching/combining-meshes)). Unity lo consiglia per le mesh vicine che non si muovono l'una rispetto all'altra, come un armadio con tanti cassetti. Se vuoi che i pezzi fermi si scartino da soli, usa lo static batching. Unisci quello che sta sempre insieme, come le gambe e il piano di un tavolo; non un'intera stanza.

Ci sono altre tre regole sugli oggetti:
- [Unity, per chi modella](https://docs.unity.com/en-us/engine/6000.3/manual/assets-and-media/asset-types/models/creating-dccassets/modeling-optimized-characters): tieni al minimo i materiali di ogni modello. Usane più di uno solo quando una parte vuole uno shader diverso, per esempio gli occhi di un personaggio.
- [Meta](https://developers.meta.com/vr/documentation/unity/unity-mobile-performance-intro/): fai attenzione al numero di oggetti. Più oggetti e più renderer ci sono nella scena, più memoria serve e più tempo ci mette Unity a scartarli e a disegnarli. E condividi le mesh dove puoi ([Meta](https://developers.meta.com/vr/documentation/unity/unity-perf/)).
- [Unity](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/optimizing-graphics-performance): per le cose lontane basta lo skybox. E disegna meno oggetti scartando di più: l'occlusion culling della lezione 6, un **Far** più corto nella camera, o una distanza di scarto per ogni layer.

**L'Instancing, un falso amico.** L'**instancing** è una funzione della GPU: disegna tante copie della stessa mesh, con lo stesso materiale, in una draw call sola, per esempio alberi o cespugli ([Unity, l'instancing](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/gpuinstancing/gpuinstancing)). Nell'Inspector di ogni materiale c'è la casella **Enable GPU Instancing**. Il nome promette tanto. Con URP però Unity consiglia di lasciarla spenta, perché aggiunge varianti allo shader ([Unity, scegliere il metodo](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/optimizing-draw-calls-choose-method)). Gli shader di URP vanno con l'SRP Batcher, e per un oggetto compatibile l'SRP Batcher passa prima dell'instancing: la casella non cambia niente, e costa una variante in più.

L'instancing serve in casi particolari. Per tantissime copie identiche, con lo stesso materiale, Unity scrive che può battere l'SRP Batcher. Ma bisogna disegnarle da script, con `Graphics.RenderMeshInstanced`, oppure togliere quegli oggetti dall'SRP Batcher, e poi controllare col Profiler che vada davvero meglio ([Unity, togliere la compatibilità con l'SRP Batcher](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/in-urp/reduce-draw-calls-urp/srpbatcher/srpbatcher-incompatible)). Unity aggiunge che il guadagno dipende dalla GPU, ed è più alto sui dispositivi mobili che sul PC. Al bar lo provi tu: con l'SRP Batcher acceso Stats non cambia.

Meta, nella sua pagina sui budget, scrive di usare l'instancing dove puoi. È un consiglio valido per qualsiasi motore. In URP, con l'SRP Batcher acceso, la casella del materiale non lo fa partire. L'instancing vero lo ottieni disegnando da script o togliendo gli oggetti dall'SRP Batcher, come qui sopra, oppure col GPU Resident Drawer, che sul Quest Unity sconsiglia.

**Chi passa prima.** Se accendi tutti i metodi, Unity sceglie così: gli oggetti statici vanno nello static batching; quelli che si muovono, con uno shader compatibile, nell'SRP Batcher; gli altri compatibili nell'instancing; il resto nel dynamic batching ([Unity, scegliere il metodo](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/optimizing-draw-calls-choose-method)).

**Forward e Forward+.** Sono due modi in cui URP calcola le luci, due percorsi di disegno. Meta dice di usare sempre Forward o Forward+ ([Meta, Best practices for VR](https://developers.meta.com/vr/documentation/unity/unity-best-practices-intro/)). Unity, per i visori autonomi, consiglia il Forward. Il Deferred, un altro percorso, fa leggere e scrivere troppa memoria alle GPU dei visori ([Unity, visori autonomi](https://docs.unity.com/en-us/engine/6000.3/manual/xr/graphics/untethered-device-optimization)). Il nostro progetto usa Forward+ sul PC e Forward sul visore.

**Il GPU Resident Drawer.** In Unity 6 c'è un sistema che usa l'instancing della GPU in automatico, per ridurre le draw call e liberare la CPU ([Unity, il GPU Resident Drawer](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/in-urp/reduce-draw-calls-urp/reduce-rendering-work-on-cpu/gpu-resident-drawer)). Ha tre condizioni: il percorso **Forward+**, una piattaforma che sa usare la GPU per i calcoli generici, i compute shader, e oggetti col **Mesh Renderer**. Con OpenGL ES non funziona. Allunga le build. Rende di più nelle scene grandi, con tante copie della stessa mesh.

Nel manuale di Unity 6.6 però c'è un avviso chiaro: evitalo sui visori autonomi come il Meta Quest. Aggiunge lavoro alla GPU e può abbassare i frame al secondo, a meno che la scena non sia frenata solo dalla CPU. In quel caso, misura col Profiler se conviene ([Unity 6.6, visori autonomi](https://docs.unity.com/en-us/engine/6000.6/manual/xr/graphics/untethered-device-optimization)). Nel nostro progetto è spento, e sul visore, che usa il Forward, non partirebbe nemmeno. Sotto c'è un'interfaccia per programmatori, `BatchRendererGroup`: Unity consiglia di usarla a mano solo nei casi avanzati.

**Il dynamic batching.** Unisce al volo, a ogni frame, le mesh piccole che si muovono e hanno lo stesso materiale: al massimo 300 vertici per mesh. Unity non lo consiglia più per la maggior parte dei casi, perché il lavoro in più della CPU può costare più della draw call che risparmia; lo consiglia solo sui dispositivi di fascia bassa ([Unity, il batching](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/draw-call-batching/draw-call-batching)). Nell'asset di URP aggiunge: se la tua GPU sa fare l'instancing, spegnilo; se sono accesi tutti e due, l'SRP Batcher ha la precedenza ([Unity, l'asset di URP](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/urp-reference/universalrp-asset)). Meta, in una pagina scritta per la pipeline vecchia, lo descrive ancora come automatico per gli oggetti che si muovono ([Meta, Android Development](https://developers.meta.com/vr/documentation/unity/unity-mobile-performance-intro/)). In URP è un'opzione dell'asset, e nel nostro progetto è spenta.

## 6. Vetri e trasparenze
**L'overdraw.** Un oggetto opaco copre quello che ha dietro. Unity disegna gli opachi dal più vicino al più lontano, e la GPU scarta i pixel nascosti col test di profondità: ogni pixel si colora una volta sola. Un oggetto trasparente invece si disegna sopra quello che c'è dietro, e mescola i colori: questa trasparenza si chiama **alpha blending**. Ogni strato di vetro ricolora gli stessi pixel. Si chiama **overdraw**, e sul visore i pixel sono tanti. Meta lo spiega così: gli oggetti trasparenti si disegnano dal più lontano al più vicino, senza test di profondità, e subiscono l'overdraw ([Meta, Android Development](https://developers.meta.com/vr/documentation/unity/unity-mobile-performance-intro/)). Le regole:
- [Meta](https://developers.meta.com/vr/documentation/unity/unity-mobile-performance-intro/): tieni l'alpha blending al minimo. Evita le geometrie trasparenti sovrapposte, come gli effetti fitti di particelle, e gli effetti a tutto schermo. Fai attenzione al fillrate, cioè a quanti pixel la GPU deve colorare.
- [Unity](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/optimizing-graphics-performance): le cause più comuni dell'overdraw sono gli elementi trasparenti sovrapposti, come l'interfaccia, le particelle e gli sprite.
- [Unity](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/draw-call-batching/draw-call-batching): gli oggetti trasparenti si mettono insieme meno degli opachi, perché Unity li ordina dal più lontano prima di raggrupparli.

**L'alpha test.** C'è un altro modo di fare i buchi: l'**alpha test**, o cutout, che ritaglia i pixel invece di mescolarli. Si usa per le foglie, le reti, le grate. Nei materiali di URP è la casella **Alpha Clipping**. Meta dice di evitarlo, perché costa molto, e di sostituirlo con l'alpha blending dove si può ([Meta, Android Development](https://developers.meta.com/vr/documentation/unity/unity-mobile-performance-intro/)). Unity spiega il perché a chi scrive shader. La GPU scarta i pixel nascosti prima di colorarli; le istruzioni che ritagliano i pixel, `discard` e `clip`, le impediscono di farlo presto. Se ti servono, accendile solo sui materiali che ne hanno bisogno ([Unity, ottimizzare gli shader](https://docs.unity.com/en-us/engine/6000.3/manual/materials-and-shaders/shaders/sl-shader-performance)). Quindi: **Alpha Clipping** acceso solo dove c'è davvero un ritaglio. Per lo stesso motivo Unity consiglia, sulle piattaforme mobili meno potenti, di spegnere il **LOD Cross Fade**, la dissolvenza fra i livelli della lezione 6, che usa l'alpha test ([Unity, configurare URP per le prestazioni](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/introduction/configure-for-better-performance)).

Per gli artisti Meta aggiunge tre cose ([Meta, Art Direction for All-in-One VR Performance](https://developers.meta.com/vr/documentation/unity/po-art-direction/) e [Meta, Balancing art and performance](https://developers.meta.com/vr/design/art-and-performance/)):
- gli alberi fatti di tante carte con l'alpha, cioè piani con una texture ritagliata, creano molto overdraw. Tieni piante e alberi statici e con una geometria più piena, oppure evita il fogliame fitto, se non è essenziale;
- per i capelli, meglio gusci scolpiti con la texture che strati di carte con l'alpha;
- qualche carta con l'alpha, dove serve, va bene: ma il costo si somma.

C'è anche un consiglio di Arm, pubblicato da Meta: l'**Alpha to Coverage** usa l'antialiasing MSAA, che vediamo nella lezione 10, per ammorbidire i bordi seghettati dei ritagli ([Meta, i consigli di Arm](https://developers.meta.com/vr/blog/top-tips-from-arm-for-vr-asset-optimization/)).

**Vedere l'overdraw.** Il **Rendering Debugger** colora i pixel secondo quante volte vengono ridisegnati ([Unity, il Rendering Debugger](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/in-urp/understand-performance/rendering-debugger/use)).
1. Premi **Play** e scegli il display del banco.
2. Dal menu in alto apri **Window ▸ Analysis ▸ Rendering Debugger**. Trascina la sua linguetta sul bordo destro della Game view e lascia andare, così le due finestre stanno affiancate.
3. Nella colonna di sinistra clicca **Rendering**. Nella riga **Overdraw Mode** scegli **Transparent**.
4. Guarda la Game view: è diventata scura, e dove ci sono strati trasparenti si accende. Più è chiaro, più strati ci sono.
5. Rimetti **Overdraw Mode** su **None** ed esci dal Play.

**La vetrinetta.** La vetrinetta dei frutti delicati ha ogni lastra doppia: chi l'ha modellata ha fatto due lastre a pochi millimetri per dare spessore al vetro. Lo strato interno costa una draw call e uno strato di overdraw, e non si vede.
1. Nella Hierarchy apri `Banco della frutta` e poi `Vetri della frutta`.
2. Seleziona le cinque lastre che finiscono con **interna**: clicca la prima e **Ctrl**+clic sulle altre.
3. Premi **Canc** per cancellarle.
4. Ora ogni lato ha un vetro solo invece di due, e la vetrinetta sembra più limpida. Per avere lo stesso vetro di prima, apri il materiale `Vetro`, clicca il colore accanto a **Base Map** e porta **A**, l'opacità, da 12 a 23. È lo stesso materiale per tutti i vetri del mercato, vetrina dei dolci compresa: un vetro un po' più spesso al posto di due sottili.
5. Salva con **Ctrl+S**.

I frutti non vanno toccati. Se la pagina del tutorial resta su **Da fare**, hai cancellato troppo o troppo poco: annulla con **Ctrl+Z** e riprova.

**Il dopo della frutta.** Premi **Play**, scegli **Display 2** e rileggi i numeri: ora **Batches** è sotto il budget della lavagna. Scrivili nel file ed esci dal Play. Fai la foto del dopo col pulsante del tutorial e salva la scena con **Ctrl+S**. Poi il curatore, come nella sezione 9.

## 7. Il laboratorio: l'atlas fatto a mano in Blender
In fondo a destra nella navata c'è il banco delle cassette regalo: dodici cassette, cinque frutti in ognuna. È un caso che troverai spesso: un modello fatto di pezzi, ognuno col suo materiale. Ogni pezzo costa una draw call, quindi ogni cassetta ne costa cinque, più il vassoio. La cura si fa dove nasce il problema, nel programma 3D. In Blender fai un **atlas**: le cinque texture diventano un'immagine sola, e i cinque frutti un oggetto solo con un materiale solo. È il lavoro che fa chi prepara i modelli per un gioco. Le cassette sono copie di un solo prefab, `Cassetta mista`, che usa il modello `cassetta_mista.fbx`: quando riesporti il modello, si aggiornano tutte.

**Il prima.** Premi **Play** e scegli **Display 6**. Leggi **Batches** e **SetPass calls**, scrivili alla riga delle cassette ed esci dal Play. Poi la foto del prima, col pulsante del tutorial.

**In Blender · importa e unisci.**
1. Apri Blender. Col mouse sulla vista 3D premi **A**, poi **X**, e scegli **Delete**: via il cubo, la camera e la luce. Se restano, finiscono selezionati coi frutti, e il bake si ferma.
2. Dal menu **File ▸ Import ▸ FBX (.fbx)** apri `cassetta_mista.fbx`. Sta nella cartella della tua copia, in `sandbox/Assets/Demo/Models/Mercato`. Blender trova da solo le texture, nella cartella `cassetta_mista.fbm` accanto al modello.
3. I cinque frutti arrivano selezionati, ma nessuno è attivo, e senza un oggetto attivo l'unione non funziona. Clicca un frutto, poi premi **A**: ora sono tutti selezionati, e quello che hai cliccato è l'attivo.
4. Premi **Ctrl+J**. Il manuale di Blender lo spiega così: **Join** unisce tutti gli oggetti selezionati in quello attivo, e ne fonde i materiali e le UV map ([Blender, Join](https://docs.blender.org/manual/en/4.2/scene_layout/object/editing/join.html)). Ora c'è un oggetto solo, col nome del frutto che hai cliccato.
5. A destra, nelle **Properties**, apri la scheda **Material**, quella con la sfera: ci sono ancora cinque materiali. Un oggetto con cinque materiali costa ancora cinque draw call, perché Unity disegna ogni materiale per conto suo.

**In Blender · una UV map nuova.** Ogni frutto usa tutta la sua texture: le sue **isole UV**, i pezzi della sua superficie stesi sul piano, occupano tutto il quadrato. Messi insieme, i cinque frutti hanno le isole una sopra l'altra. Serve una seconda UV map, dove le isole stanno una accanto all'altra.
1. Nelle **Properties** apri la scheda **Object Data**, quella col triangolo verde, e apri il riquadro **UV Maps**. C'è `UVMap`.
2. Premi **+**. Il **+** copia la UV map selezionata: nasce `UVMap.001`, evidenziata, con le isole ancora una sopra l'altra. Fai doppio clic su `UVMap.001` e chiamala `Atlas`.
3. Guarda l'icona della fotocamera: resta su `UVMap`. La fotocamera segna la UV map che i materiali leggono quando disegnano ([Blender, UV Maps](https://docs.blender.org/manual/en/4.2/modeling/meshes/uv/uv_texture_spaces.html)). Quella evidenziata, `Atlas`, è quella su cui lavori, e quella in cui il bake scriverà.
4. In alto passa alla scheda **UV Editing**: Blender entra da solo in Edit Mode. Col mouse sulla vista 3D, a destra, premi **A**: tutto selezionato.
5. Col mouse sull'editor UV, a sinistra, premi **A**: anche lì tutto selezionato. Senza questo passo il comando dopo non fa niente.
6. Nell'editor UV apri **UV ▸ Pack Islands**. Nella finestrella metti **Margin** a `0.005` e premi **OK**. Pack Islands sposta, scala e gira le isole per riempire il quadrato, e lascia il margine fra l'una e l'altra ([Blender, Pack Islands](https://docs.blender.org/manual/en/4.2/modeling/meshes/uv/editing.html)). Le isole dei cinque frutti ora non si sovrappongono più.
7. Col mouse sulla vista 3D premi **Tab** per tornare in Object Mode.

Il margine lascia qualche pixel fra un'isola e l'altra. Senza, da lontano i colori di due frutti si mescolano sui bordi.

**In Blender · l'immagine dell'atlas.** Ora prepari l'immagine vuota in cui Blender disegnerà le cinque texture.
1. Nell'editor a sinistra apri **Image ▸ New**. Chiamala `cassetta_mista_atlas`, lascia **1024** per **1024**, togli la spunta ad **Alpha** e premi **OK**.
2. In alto passa alla scheda **Shading**. A destra, nella scheda **Material**, clicca il primo dei cinque materiali.
3. Col mouse sull'editor dei nodi, in basso, premi **Shift+A**, scegli **Texture ▸ Image Texture** e posa il nodo in un punto vuoto. Nel nodo apri il menu delle immagini e scegli `cassetta_mista_atlas`. Non collegarlo a niente.
4. Fai lo stesso per gli altri quattro materiali.

Il nodo che hai posato per ultimo è quello attivo, col bordo chiaro. Il manuale di Blender: il bake scrive nell'immagine del nodo **Image Texture** attivo di ogni materiale ([Blender, Bake](https://docs.blender.org/manual/en/4.2/render/cycles/baking.html)).

**In Blender · il bake del colore.** Il **bake** è la cottura: Blender guarda ogni frutto con la sua texture e la sua UV map di sempre, e disegna quello che vede nell'immagine nuova, seguendo la UV map `Atlas`. Per il bake serve **Cycles**, il motore di render di Blender che segue i raggi di luce.
1. Nelle **Properties** apri la scheda **Render**, quella con la macchina fotografica sul retro. In **Render Engine** scegli **Cycles**.
2. Sotto **Sampling ▸ Render** metti **Max Samples** a `16`. Blender usa le impostazioni del render anche per il bake. Per copiare un colore bastano pochi campioni, e il bake va molto più veloce.
3. Più in basso apri il riquadro **Bake**. In **Bake Type** scegli **Diffuse**. Sotto **Contributions** togli la spunta a **Direct** e **Indirect** e lascia **Color**. Il manuale di Blender: con solo **Color** ottieni il colore della superficie, senza luci né ombre. **Margin** resta a 16 px.
4. Nella vista 3D il frutto unito deve essere selezionato. Premi **Bake**. In pochi secondi l'immagine si riempie dei cinque frutti.
5. Torna alla scheda **UV Editing**: a sinistra c'è l'atlas, ora pieno dei frutti. Scegli **Image ▸ Save As**, il formato **PNG**, la cartella `sandbox/Assets/Demo/Models/Mercato` e il nome `cassetta_mista_atlas.png`. Nella scheda Shading l'editor delle immagini non mostra l'atlas, e Save As resta grigio.

Blender non salva da solo le immagini cotte. Se chiudi senza salvarle, le perdi. Il margine del bake allarga il colore di ogni isola di qualche pixel: il manuale di Blender spiega che evita gli stacchi sulle cuciture, dovuti al filtro delle texture e alle mipmap.

**In Blender · il bake della normal map.** Anche il rilievo dei frutti, che sta nelle normal map, deve finire in un'immagine sola. La **normal map** è la texture che dice alla luce come è inclinata la superficie punto per punto.
1. Nella scheda **UV Editing**, **Image ▸ New**: chiamala `cassetta_mista_atlas_normal`, 1024 per 1024, senza **Alpha**, **OK**. Poi torna alla scheda **Shading**.
2. In ognuno dei cinque materiali, nel nodo **Image Texture** che hai posato, scegli `cassetta_mista_atlas_normal` al posto dell'altra immagine.
3. In uno di quei nodi metti **Color Space** su **Non-Color**. Vale per l'immagine, quindi basta una volta. Una normal map non è un colore: sono direzioni, e non vanno corrette come i colori ([Blender, il nodo Normal Map](https://docs.blender.org/manual/en/4.2/render/shader_nodes/vector/normal_map.html)).
4. Nel riquadro **Bake** scegli **Bake Type: Normal**. **Space** resta **Tangent**: il manuale di Blender dice che è la scelta giusta quasi sempre. Premi **Bake**.
5. Torna alla scheda **UV Editing**: l'editor a sinistra mostra `cassetta_mista_atlas_normal`. **Image ▸ Save As**, PNG, stessa cartella, nome `cassetta_mista_atlas_normal.png`.

**In Blender · un materiale solo.**
1. Nella scheda **Object Data**, in **UV Maps**, clicca `UVMap` e premi **−**. Resta solo `Atlas`, ora con la fotocamera.
2. Nella scheda **Material** clicca il secondo materiale e premi **−** quattro volte. Resta un materiale solo, quello del frutto che avevi cliccato. Fai doppio clic sul suo nome e chiamalo `Cassetta atlas`.
3. Nell'editor dei nodi trovi il nodo della texture del frutto collegato a **Base Color**: nel suo menu scegli `cassetta_mista_atlas`. Nel nodo collegato al **Normal Map** scegli `cassetta_mista_atlas_normal`. Il nodo che avevi posato per il bake ora non serve: selezionalo e premi **X**.
4. Col mouse sulla vista 3D premi **Z** e scegli **Material Preview**: i cinque frutti devono sembrare quelli di prima.

**In Blender · esporta.** Ogni volta, come nella lezione 6 ([Blender, FBX](https://docs.blender.org/manual/en/4.2/addons/import_export/scene_fbx.html)):
1. Con il frutto unito selezionato, **File ▸ Export ▸ FBX (.fbx)**.
2. A destra, sotto **Include**, spunta **Selected Objects**: esporti solo il frutto unito. Sotto **Transform**, in **Apply Scalings**, scegli **FBX Units Scale**: se no il modello arriva in Unity cento volte più piccolo.
3. Salva sopra `cassetta_mista.fbx`, nella cartella `sandbox/Assets/Demo/Models/Mercato`, con lo stesso nome.

Se qualcosa è andato storto in Blender, ricomincia in un file nuovo: **File ▸ New ▸ General**, e importa di nuovo il modello. Se l'hai già sovrascritto, nella cartella della tua copia lancia `git checkout -- sandbox/Assets/Demo/Models/Mercato/cassetta_mista.fbx` e il modello torna com'era.

**In Unity · il materiale dell'atlas.** Torna in Unity. Le cassette si aggiornano da sole: ora ognuna ha un oggetto solo al posto dei cinque frutti, ma con un materiale che non va bene.
1. Nella finestra **Project** apri `Assets/Demo/Models/Mercato` e seleziona `cassetta_mista_atlas_normal`. Nell'Inspector metti **Texture Type** su **Normal map** e premi **Apply**.
2. Nella stessa cartella clicca col destro e scegli **Create ▸ Material**. Chiamalo `Cassetta atlas`.
3. Nell'Inspector del materiale, nel menu **Shader**, scegli **Universal Render Pipeline ▸ Simple Lit**. Trascina `cassetta_mista_atlas` sul quadratino di **Base Map** e `cassetta_mista_atlas_normal` su quello di **Normal Map**. Metti la spunta a **Specular Highlights**.
4. Apri `Assets/Samples/RUFA Core/Lezione 07 - Draw call/Prefab` e fai doppio clic su `Cassetta mista`. Nella Hierarchy apri `Cassetta mista` e seleziona il frutto unito: si chiama come il frutto che avevi cliccato in Blender, per esempio `Frutto 2`.
5. Nell'Inspector, nel **Mesh Renderer**, alla voce **Materials**, trascina `Cassetta atlas` sopra il materiale che c'è.
6. Esci dal prefab con la freccia in alto a sinistra nella Hierarchy. Il materiale va nel prefab, non su una cassetta della scena: così vale per tutte e dodici.

**Le cassette · il dopo.** Ora che le cassette hanno un materiale solo, lo static batching le può mettere insieme.
1. Nella Hierarchy apri `Banco delle cassette` e seleziona `Cassette`. Nell'Inspector apri la freccia di **Static**, scegli **Batching Static** e rispondi **Yes, change children**.
2. Premi **Play**, scegli **Display 6** e rileggi i numeri. Scrivili nel file ed esci dal Play.
3. Fai la foto del dopo e mostrala al curatore insieme a quella del prima.
4. Salva la scena con **Ctrl+S**.

Nel file delle misure scrivi anche quanto è grande il tuo atlas e quanto margine hai lasciato.

Il modello riesportato, le due immagini e il materiale `Cassetta atlas` stanno in `Assets/Demo/Models/Mercato`: al prossimo `git aggiorna` quella cartella torna come prima. È un esercizio: le tue foto e il file delle misure restano. L'atlas per il tuo progetto fallo sui tuoi modelli, nella cartella del progetto.

## 8. Trucchi del mestiere
Nel programma 3D:
- **Un oggetto, un materiale.** Ogni materiale in più su un oggetto è una draw call in più. Quando puoi, fai un materiale solo e metti le differenze nella texture. Unity lo chiede a chi modella: più materiali solo se una parte vuole uno shader diverso.
- **La palette.** Per i modelli a colori pieni basta una texture piccola con dei quadretti di colore. Ogni isola UV si posa sul suo quadretto. Tanti modelli, un materiale, e lo static batching li mette insieme. È anche il modo più semplice di seguire il consiglio di Meta: cambiare colore senza cambiare materiale.
- **Le trim sheet.** Una texture a strisce, ognuna con un dettaglio che si ripete: una cornice, un bordo di metallo, una fila di mattoni. Le pareti e i mobili di una stanza ci posano sopra le loro isole e condividono un materiale. In una escape room è il modo più comodo per arredare tante stanze con pochi materiali.
- **Unisci ciò che sta sempre insieme.** Le gambe e il piano di un tavolo sì. Un oggetto che si muove, si raccoglie o si spegne no: deve restare separato. Una stanza intera nemmeno: perderesti lo scarto dei pezzi che non si vedono.
- **La densità dell'atlas.** Dai a ogni isola uno spazio proporzionato a quanto si vedrà. Un litchi non ha bisogno di mezzo atlas; quello che il giocatore prende in mano sì, perché in VR arriva a pochi centimetri dagli occhi. In Blender **UV ▸ Average Island Scale** porta le isole alla stessa scala, poi Pack Islands le impacchetta; dopo puoi ingrandire a mano quelle che contano.
- **I margini.** Lascia qualche pixel fra le isole e cuoci con un margine. Da lontano Unity usa versioni più piccole della texture, le **mipmap**, e senza margine i bordi prendono il colore del vicino.
- **Salva le immagini cotte.** Blender non le salva da solo.
- **Le normal map.** In **Non-Color**, sempre. Blender e Unity usano la stessa convenzione, quella di OpenGL, col verde verso l'alto: Unity la chiama Y+ ([Unity, le normal map](https://docs.unity.com/en-us/engine/6000.3/manual/materials-and-shaders/built-in/shader-built-in-configure-properties/standard-shader-texture-maps/material-parameter-normal-map/map)). Una normal map fatta per DirectX ha il verde al contrario: il rilievo sembra scavato, e in Unity si corregge invertendo il verde.
- **Il dettaglio da un modello pesante.** Nel riquadro **Bake** c'è **Selected to Active**: cuoce sull'oggetto attivo il dettaglio degli oggetti selezionati. Così il rilievo di una scultura finisce nella normal map della versione leggera, come nella retopologia della lezione 6.
- **Prova senza texture.** Meta lo suggerisce: con gli shader giusti un gioco può essere bellissimo anche senza texture. Uno stile non realistico, low poly o a colori pieni, toglie pressione tecnica e lascia risorse ad altro ([Meta, Art direction](https://developers.meta.com/vr/design/art-direction/)).
- **Il budget prima di cominciare.** Meta consiglia di scrivere i budget degli asset nella guida di stile del progetto: poligoni per modello, texture, LOD ([Meta, Art preproduction](https://developers.meta.com/vr/design/art-preproduction/)). Aggiungi i materiali per modello.

In Unity:
- **Prima il Frame Debugger, poi la cura.** Ti dice quali draw call ci sono e perché un gruppo si interrompe.
- **Pochi shader, con le stesse opzioni.** Un Simple Lit per quasi tutto. Ogni opzione accesa su un materiale solo è una variante in più. Per controllare, seleziona i materiali insieme: un trattino in una casella vuol dire opzioni diverse.
- **Ordina per materiale.** Meta consiglia di ordinare gli oggetti per materiale, così cambia poco fra una draw call e l'altra. Il modo più semplice di aiutare Unity è avere pochi materiali.
- **`sharedMaterial`, non `material`.** In uno script `material` crea una copia del materiale per quell'oggetto, e la copia rompe lo static batching. Per cambiare il materiale di tutti usa `sharedMaterial`.
- **Niente `MaterialPropertyBlock` con l'SRP Batcher.** Lo interrompe. Unity consiglia materiali diversi per proprietà diverse.
- **Instancing spento.** Con l'SRP Batcher acceso non serve e aggiunge varianti.
- **Statico solo ciò che è fermo.** Un oggetto che si afferra, una porta che si apre, un cassetto: non vanno segnati **Batching Static**, perché uniti agli altri non si muovono più.
- **Pochi oggetti.** Meta lo ricorda: ogni oggetto e ogni renderer costano memoria e tempo, anche solo per essere scartati. Un fondale lontano può stare nello skybox.
- **Poche camere.** Unity dice che ogni camera costa a URP lo scarto e il disegno, e di usarne meno possibile ([Unity, configurare URP per le prestazioni](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/introduction/configure-for-better-performance)). Meta chiede una sola vista per occhio e attenzione alle camere che ripuliscono lo schermo ([Meta, Android Development](https://developers.meta.com/vr/documentation/unity/unity-mobile-performance-intro/)). Le camere fisse del mercato hanno il tag `EditorOnly`: nell'app per il visore non ci sono.
- **Le luci con le ombre.** Ogni luce che fa ombra ridisegna gli oggetti nella passata delle ombre, e le draw call si moltiplicano. In Stats, **Shadow casters** conta gli oggetti che fanno ombra. Meta: le ombre dinamiche spesso ridisegnano la scena più volte nello stesso frame, e quando ti avvicini al limite delle draw call conviene spegnerle ([Meta, Art Direction for All-in-One VR Performance](https://developers.meta.com/vr/documentation/unity/po-art-direction/) e [Meta, Best practices for VR](https://developers.meta.com/vr/documentation/unity/unity-best-practices-intro/)). Per questo il mercato non ha ombre in tempo reale. Lo vediamo nella lezione 8.
- **Pochi strati trasparenti.** Un vetro al posto di due, niente alpha test dove puoi, niente effetti a tutto schermo.
- **La misura vera è sul visore.** Stats sul PC conta le passate del PC. Per i numeri del Quest servono il Profiler collegato al visore e OVR Metrics, come nella lezione 5.

## 9. Il giro del curatore
Per ogni banco il giro è lo stesso della frutta:
1. in Play, il display del banco, e i numeri nel file delle misure;
2. la foto del prima, col pulsante del tutorial;
3. la cura, fuori dal Play;
4. di nuovo in Play i numeri nel file, e la foto del dopo.

Le regole del curatore valgono sempre. Non spegnere, non cancellare e non spostare la merce: un mercato senza frutta non è un mercato ottimizzato. E la foto del dopo deve sembrare quella del prima. Poi un compagno fa il **curatore**, come nella lezione 6. Gli mostri le due foto a tutto schermo, nell'ordine che decidi tu e senza fargli vedere i nomi dei file. Lui dice qual è quella curata, e perché. Se ti indica una differenza che c'è davvero, per esempio un frutto meno lucido o un vetro più chiaro, torna al banco e regola: la **Smoothness** di un materiale, l'**Alpha** del vetro. Se tira a indovinare, hai fatto un buon lavoro. Poi scambiatevi i ruoli, e scrivi nel file cosa ha notato e cosa hai cambiato.

I tre banchi della verifica stanno nel mercato. La lista del tutorial controlla la scena aperta: deve essere `Mercato`. Alla fine di ogni banco salva con **Ctrl+S**.
- **Le spezie**, **Display 3**, budget 110 Batches. Ogni mucchio e ogni ciotola hanno la loro copia del materiale, e le spezie usano shader e opzioni diverse. Un materiale per spezia e uno per le ciotole, tutti su Simple Lit con le stesse opzioni, e tutto fermo. Sono polveri opache: niente riflessi. Nella ricerca della Hierarchy scrivi il nome intero: `Pepe` trova sia il pepe nero sia il pepe verde.
- **La vetrina dei dolci**, **Display 4**. Qui il budget non è in draw call: il cliente vuole al massimo tre strati di vetro uno dietro l'altro, e li guardi nel Rendering Debugger. Ogni lastra della vetrina è doppia, il ripiano di vetro pure, e ogni campana sopra le torte ne ha un'altra dentro. Togli gli strati che non servono, senza toccare i dolci.
- **Il bar**, **Display 5**, budget 60 Batches. Tavolini e sedie tutti uguali, ognuno con la sua copia del materiale. Un materiale per le sedie e uno per i tavolini. Poi prova l'Instancing: spunta **Enable GPU Instancing** nel materiale `Sedia`, guarda **Batches** in Play, togli di nuovo la spunta e scrivi nel file cosa hai visto. La cura vera è lo static batching: sedie e tavolini non si muovono.

## 10. La consegna
Tutto in `Consegne/L07/`:
- `misure.md`, dal pulsante del tutorial, senza nessun «da scrivere»;
- le foto del mercato: `frutta-`, `spezie-`, `vetrina-` e `bar-prima.jpg` e `-dopo.jpg`;
- le foto del laboratorio: `cassette-prima.jpg` e `cassette-dopo.jpg`;
- le foto del tuo progetto: `progetto-prima.jpg` e `progetto-dopo.jpg`, dalla tua `Camera foto`.

Il file delle misure ha tre parti. Il mercato: i numeri di ogni banco prima e dopo, le SetPass con l'SRP Batcher spento, e cosa ha notato il curatore. Il laboratorio: le cassette prima e dopo, la grandezza del tuo atlas e il margine. Il tuo progetto: dove hai messo la camera e perché, i numeri prima e dopo, e cosa hai fatto.

**Il tuo progetto.** Il pulsante del tutorial apre la scena del tuo blockout. Usa la `Camera foto` che hai messo nella lezione 6, sul **Display 2**. Se non c'è, rifalla:
1. Nella Scene view inquadra il punto che conta di più, all'altezza degli occhi.
2. Dal menu in alto scegli **GameObject ▸ Camera** e rinomina la camera `Camera foto`.
3. Con la camera selezionata scegli **GameObject ▸ Align With View**: la camera prende l'inquadratura della Scene view.
4. Nell'Inspector della camera metti **Target Display** su **Display 2**.

Poi premi **Play**, scegli **Display 2**, leggi i tre numeri e scrivili nella parte del progetto. Esci dal Play e fai la foto col pulsante del tutorial. Usa le cure di oggi dove servono:
- con il Frame Debugger, trova i materiali e gli shader che interrompono i gruppi;
- un materiale solo per gli oggetti uguali, e niente copie;
- un solo shader mobile, Simple Lit, con le stesse opzioni;
- **Batching Static** su tutto quello che non si muove, e su niente di quello che si afferra o si apre;
- meno strati trasparenti, e niente alpha test dove puoi;
- se modelli, un atlas per i tuoi oggetti fatti di pezzi.

Il budget del tuo progetto è quello di Meta per il tuo tipo di gioco. Se il tuo blockout è già sotto, va bene: la consegna chiede di misurare e di scrivere cosa hai fatto e perché.

A fine lezione, `git salva "Lezione 07"`.

## 11. I budget nella vita vera
- **Meta**, per i visori: sul Quest 3 e sul 3S da 200 a 300 draw call per frame con tanta simulazione, da 400 a 600 con una simulazione media, da 700 a 1000 con poca. Sul Quest 2 e sul Quest Pro i numeri sono più bassi: da 80 a 200, da 200 a 300, da 400 a 600 ([Meta, Testing and performance analysis](https://developers.meta.com/vr/documentation/unity/unity-perf/)).
- **VRChat**, avatar per telefoni, tablet e Meta Quest: per i **Material Slots**, i posti per i materiali sulle mesh dell'avatar, 1 è Excellent, 1 Good, 2 Medium, 4 Poor; oltre è Very Poor. Per le mesh vale lo stesso: al massimo una mesh con lo scheletro e una semplice per Excellent e Good, due e due per Medium e Poor. VRChat spiega che ogni posto per un materiale crea una parte della mesh in più, e quindi altre draw call. Su questi dispositivi, con l'impostazione di base, gli avatar Poor e Very Poor non si vedono: al loro posto compare un avatar di riserva, il Fallback. Sul PC i limiti sono più larghi: 4 materiali per Excellent, 32 per Poor ([VRChat, i ranghi degli avatar](https://creators.vrchat.com/avatars/avatar-performance-ranking-system/)).
- **Lo studio.** Ti dirà quanti materiali può avere il tuo modello prima ancora che tu cominci: è uno dei budget della guida di stile.

## 12. Problemi comuni
- **Il primo compito resta Da fare.** Le merci del mercato non ci sono ancora: chiudi Unity, lancia `git aggiorna` un'altra volta e riapri. Unity Hub deve essere aperto.
- **Quello che hai cambiato è sparito.** L'hai cambiato in Play, e quando esci dal Play Unity rimette tutto com'era. Misuri in Play, curi fuori dal Play.
- **Il menu dei Display non risponde in Play.** Il rig ha preso il mouse: premi **Esc**.
- **Batches non scende dopo lo static batching.** Stai leggendo fuori dal Play, oppure gli oggetti hanno ancora copie diverse del materiale. Lo static batching unisce solo gli oggetti con lo stesso materiale: fondi le copie, poi rileggi in Play.
- **SetPass non scende.** Un materiale ha ancora un'opzione diversa dagli altri, o uno shader diverso. Selezionali tutti e cerca i trattini nelle caselle.
- **In Play le copie dei materiali tornano.** Uno script usa `material`: a ogni avvio crea una copia per oggetto, e nell'Inspector il materiale ha il nome seguito da **(Instance)**. Nello script usa `sharedMaterial`.
- **Un oggetto non si muove più.** È segnato **Batching Static**: in Play il suo disegno resta dov'era. Togli il flag a tutto quello che si afferra, si apre o si sposta.
- **Il compito resta Da fare, e la cura è giusta.** Nel banco qualcosa è stato spostato, spento o cancellato: annulla con **Ctrl+Z**, o chiamami.
- **Il pepe verde è diventato nero.** Hai cercato solo `Pepe` e hai preso tutti e due: annulla con **Ctrl+Z** e cerca il nome intero.
- **I tuoi numeri sono la metà di quelli del compagno, e l'esperimento dell'SRP Batcher non cambia niente.** Il tuo editor è rimasto su Android dopo una build per il visore: disegna col livello Mobile, senza la passata dell'SSAO e con un altro asset di URP. **File ▸ Build Profiles**, **Windows**, **Switch Platform**: la pagina «L'editor sul PC» del tutorial lo controlla.
- **Ctrl+J non fa niente.** Nessun oggetto è attivo: clicca un frutto, premi **A**, poi **Ctrl+J**.
- **Pack Islands non fa niente.** Nell'editor UV non c'era niente di selezionato: col mouse sull'editor UV premi **A**, poi di nuovo **UV ▸ Pack Islands**.
- **Blender dice No active image found in material.** In un materiale manca il nodo **Image Texture** con l'immagine dell'atlas, o non è il nodo attivo: posalo, scegli l'immagine e cliccalo.
- **L'immagine cotta è sparita.** Non l'hai salvata: Blender non salva da solo le immagini cotte. Rifai il bake e poi **Image ▸ Save As**.
- **Il rilievo dei frutti è strano.** La normal map: in Blender ci vuole **Non-Color** prima del bake; in Unity ci vuole **Texture Type: Normal map** e poi **Apply**.
- **Le cassette sono minuscole.** Hai esportato senza **Apply Scalings: FBX Units Scale**: rifai l'esportazione.
- **Il bake si ferma su una camera o una luce.** All'inizio non hai cancellato tutto: seleziona la camera e la luce, **X**, **Delete**, poi riselezioni il frutto unito e rifai il bake.
- **Le cassette non cambiano.** Hai salvato con un altro nome o in un'altra cartella: il file deve essere `cassetta_mista.fbx`, in `sandbox/Assets/Demo/Models/Mercato`.
- **La pagina del materiale dell'atlas resta Da fare.** Il modello ha ancora più parti, perché in Blender manca l'unione o un materiale. Oppure il frutto unito ha ancora un materiale con una texture vecchia, o due materiali. Oppure hai messo il materiale su una cassetta della scena e non nel prefab.
- **La foto è nera o vuota.** Premi di nuovo il pulsante: subito dopo aver aperto la scena, la prima foto può uscire senza oggetti.
- **Il mercato è rovinato.** Cancella la cartella `Assets/Samples/RUFA Core/Lezione 07 - Draw call` e premi di nuovo **Apri il mercato**. Le cure fatte lì dentro si perdono; le foto e il file delle misure restano.

## Fonti
- Meta, «Testing and performance analysis» per Unity, i budget delle draw call per Quest e i cambi di stato: https://developers.meta.com/vr/documentation/unity/unity-perf/
- Meta, «Draw Call Cost Analysis», il costo dei cambi di shader, materiale e mesh: https://developers.meta.com/vr/documentation/unity/po-draw-call-analysis/
- Meta, «Android Development», le regole per le app autonome (batching, shader, atlas, overdraw, alpha test): https://developers.meta.com/vr/documentation/unity/unity-mobile-performance-intro/
- Meta, «Best practices for VR» (Forward e Forward+, SSAO, ombre, passate): https://developers.meta.com/vr/documentation/unity/unity-best-practices-intro/
- Meta, «Art Direction for All-in-One VR Performance» (carte con l'alpha, fogliame, capelli, ombre dinamiche): https://developers.meta.com/vr/documentation/unity/po-art-direction/
- Meta, «Balancing art and performance»: https://developers.meta.com/vr/design/art-and-performance/
- Meta, «Art direction» e «Art preproduction» (lo stile, i budget degli asset): https://developers.meta.com/vr/design/art-direction/ e https://developers.meta.com/vr/design/art-preproduction/
- Meta, i consigli di Arm sugli asset (Alpha to Coverage): https://developers.meta.com/vr/blog/top-tips-from-arm-for-vr-asset-optimization/
- Unity 6.3, le draw call: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/optimizing-draw-calls
- Unity 6.3, scegliere il metodo per ridurre le draw call: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/optimizing-draw-calls-choose-method
- Unity 6.3, l'SRP Batcher in URP: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/in-urp/reduce-draw-calls-urp/srpbatcher/srpbatcher
- Unity 6.3, accenderlo, la compatibilità, toglierla, controllarlo nel Frame Debugger: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/in-urp/reduce-draw-calls-urp/srpbatcher/srpbatcher-enable , https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/in-urp/reduce-draw-calls-urp/srpbatcher/srpbatcher-materials , https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/in-urp/reduce-draw-calls-urp/srpbatcher/srpbatcher-incompatible , https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/in-urp/reduce-draw-calls-urp/srpbatcher/srpbatcher-profile
- Unity 6.3, il batching, prepararlo, lo static batching nella build e mentre il gioco gira, le proprietà delle mesh unite, unire le mesh a mano: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/draw-call-batching/draw-call-batching , https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/draw-call-batching/set-up , https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/draw-call-batching/enable , https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/draw-call-batching/static-batching-enable , https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/draw-call-batching/properties , https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/draw-call-batching/combining-meshes
- Unity 6.3, l'instancing: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/reduce-draw-calls/gpuinstancing/gpuinstancing
- Unity 6.3, il GPU Resident Drawer: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/in-urp/reduce-draw-calls-urp/reduce-rendering-work-on-cpu/gpu-resident-drawer
- Unity 6.6, i visori autonomi, con l'avviso sul GPU Resident Drawer: https://docs.unity.com/en-us/engine/6000.6/manual/xr/graphics/untethered-device-optimization
- Unity 6.3, i visori autonomi (Forward, SSAO): https://docs.unity.com/en-us/engine/6000.3/manual/xr/graphics/untethered-device-optimization
- Unity 6.3, scegliere uno shader di URP e i modelli di luce: https://docs.unity.com/en-us/engine/6000.3/manual/materials-and-shaders/built-in/shaders-in-universalrp/choose e https://docs.unity.com/en-us/engine/6000.3/manual/materials-and-shaders/built-in/shaders-in-universalrp/shading-model
- Unity 6.3, configurare URP per le prestazioni (Complex Lit, Baked Lit e Simple Lit, SRP Batcher, camere, LOD Cross Fade): https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/introduction/configure-for-better-performance
- Unity 6.3, l'asset di URP (SRP Batcher, Dynamic Batching, GPU Resident Drawer): https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/urp-reference/universalrp-asset
- Unity 6.3, ottimizzare gli shader (discard e clip): https://docs.unity.com/en-us/engine/6000.3/manual/materials-and-shaders/shaders/sl-shader-performance
- Unity 6.3, ridurre il lavoro di CPU e GPU (oggetti, scarto, overdraw): https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/optimizing-graphics-performance
- Unity 6.3, creare modelli per le prestazioni (pochi materiali): https://docs.unity.com/en-us/engine/6000.3/manual/assets-and-media/asset-types/models/creating-dccassets/modeling-optimized-characters
- Unity 6.3, la finestra Statistics: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/profiling/rendering-statistics
- Unity 6.3, il Frame Debugger e i dettagli di un disegno: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/profile-rendering/frame-debugger/debug e https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/profiling/frame-debugger-window-event-information
- Unity 6.3, il Rendering Debugger: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/in-urp/understand-performance/rendering-debugger/use
- Unity 6.3, il modulo Rendering del Profiler: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/profile-rendering/profiler-rendering
- Unity 6.3, le normal map (Y+): https://docs.unity.com/en-us/engine/6000.3/manual/materials-and-shaders/built-in/shader-built-in-configure-properties/standard-shader-texture-maps/material-parameter-normal-map/map
- Blender 4.2, Join: https://docs.blender.org/manual/en/4.2/scene_layout/object/editing/join.html
- Blender 4.2, le UV map (la fotocamera, il pulsante +): https://docs.blender.org/manual/en/4.2/modeling/meshes/uv/uv_texture_spaces.html
- Blender 4.2, Pack Islands e Average Island Scale: https://docs.blender.org/manual/en/4.2/modeling/meshes/uv/editing.html
- Blender 4.2, il bake con Cycles: https://docs.blender.org/manual/en/4.2/render/cycles/baking.html
- Blender 4.2, il nodo Image Texture e il nodo Normal Map: https://docs.blender.org/manual/en/4.2/render/shader_nodes/textures/image.html e https://docs.blender.org/manual/en/4.2/render/shader_nodes/vector/normal_map.html
- Blender 4.2, importare ed esportare FBX: https://docs.blender.org/manual/en/4.2/addons/import_export/scene_fbx.html
- VRChat, i ranghi degli avatar: https://creators.vrchat.com/avatars/avatar-performance-ranking-system/
