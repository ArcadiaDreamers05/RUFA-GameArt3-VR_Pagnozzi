Lezione 5 · Profiling e frame budget. Le note prima del primo capitolo non si vedono nella finestra. Capitoli insieme 0-5 circa 92' per lo studente lento e verifica circa 46', scelti da Gianluca il 01/11; la stima è 97' e 45' (tools/stima-tempi.py, righe "Insieme" e "Verifica"). Parla Gianluca in prima persona e dà del tu allo studente. Nessun numero misurato su un PC: solo quelli di Meta e di Unity, e quelli costruiti nelle scene.

# 0 · Prima di iniziare

## Cosa farai in questa lezione
Oggi impariamo a misurare: quanto costa un frame, chi non ce la fa, la CPU o la GPU, e perché. Prima di ottimizzare qualunque cosa, serve questo.
Lo facciamo insieme sul demo, la casa con tutti i difetti che cureremo dalla lezione 6 alla 10. Poi, da solo, trovi i colpevoli in una palestra e misuri il tuo blockout sul PC e sul visore, con un report. Il metodo vale per qualunque progetto Unity, anche fuori dalla VR.

## I mattoncini nuovi
Questa pagina controlla che nel tuo progetto ci siano i mattoncini della lezione 3: leva, mobile, manovella, manopola, slider, interruttore e tastierino, in `Assets/RUFA/Prefabs`.
Se qui sotto c'è scritto **Fatto**, vai avanti. Se resta **Da fare**, chiudi Unity, lancia `git aggiorna` un'altra volta nella cartella della tua copia e riapri il progetto. Il primo `git aggiorna` di oggi porta il comando nuovo, il secondo porta i file.
> verifica: asset-esiste Assets/RUFA/Prefabs/Manovella.prefab

## Gli strumenti da installare
Oggi usiamo tre strumenti di Unity che il progetto non ha ancora. Si installano dal Package Manager, fuori dal Play.
1. Dal menu in alto apri **Window ▸ Package Manager**.
2. Nella colonna di sinistra clicca **Unity Registry**.
3. Nella casella di ricerca in alto scrivi `Profile Analyzer` e clicca il package nella lista.
4. A destra premi **Install**. Unity scarica il package e ricompila: aspetta che la rotellina in basso a destra si fermi.
5. Fai lo stesso con `Memory Profiler` e con `Project Auditor`.
Quando ci sono tutti e tre, qui sotto compare **Fatto**. Se un'installazione non parte, controlla che Unity Hub sia aperto e connesso al tuo account, poi riprova. Intanto guarda il proiettore: ti spiego il budget.
> verifica: pacchetto com.unity.performance.profile-analyzer | com.unity.memoryprofiler | com.unity.project-auditor
> attesa: 60 Le installazioni
> lavoro: 3 2

## Il rig del PC
Oggi non c'è niente da prendere in mano: ti muovi col rig del PC, con tastiera e mouse.
1. Dal menu in alto apri **RUFA ▸ Play mode**. Se accanto a **Simulatore VR** c'è la spunta, clicca la voce: la spunta sparisce.
In Play clicca nella Game view per guardare col mouse e cammina con **W A S D**. **Esc** ti ridà il mouse.
> verifica: simulatore-spento

## Apri il demo
Il paziente di oggi è il demo, nella tua copia del progetto. Premi il pulsante qui sotto: si apre la scena `Assets/Demo/Scenes/Demo.unity`.
Tutte le prove sul demo le facciamo in Play: quando esci dal Play, la scena torna com'era.
> azione: Apri il demo | apri-scena-progetto | Assets/Demo/Scenes/Demo.unity
> verifica: scena-aperta Demo

## Le condizioni della misura
In editor una misura vale solo a tre condizioni. Mettiamole adesso, una volta per tutte.
1. La Game view deve stare davanti. Se è nella stessa finestra della Scene view, clicca la linguetta **Game**. Una Game view nascosta non disegna il gioco, e i tempi diventano quelli di una scena vuota.
2. In alto nella Game view apri il menu delle proporzioni, quello con scritto **Free Aspect**, e togli la spunta a **VSync (Game view only)**. Con la spunta, l'editor aspetta il monitor a ogni frame: su uno schermo a 60 Hz nessun frame scenderebbe sotto i 16,7 ms, nemmeno con la scena vuota.
3. La terza riguarda il Profiler, che apriamo nel capitolo 2: mentre registra, i tempi crescono. I tempi si leggono col Profiler fermo.
> verifica: vsync-spento

## Il frame in VR
Lo schermo del Quest 3S si aggiorna 72, 90 o 120 volte al secondo, e il nostro progetto chiede 90. Se il frame nuovo non è pronto, il visore ripete il vecchio, spostato per seguire la testa: l'immagine segue lo sguardo, ma le cose che si muovono fanno uno scatto. È il **judder**, e nel visore lo senti nello stomaco. OVR Metrics, lo strumento di Meta del capitolo 5, chiama questi frame ripetuti **stale frame**.

## Il budget
A 90 Hz ogni frame ha **11,1 ms**: è il nostro budget. CPU e GPU lavorano in parallelo: mentre la GPU disegna un frame, la CPU prepara il prossimo. Ognuna ha tutti gli 11,1 ms, e se una delle due li supera il frame arriva tardi. Se sfora la CPU il gioco è **CPU-bound**, se sfora la GPU è **GPU-bound**, e le cure sono diverse.
Le regole dello Store di Meta: l'app fa almeno 60 fps, con lo schermo almeno a 72 Hz; entro 4 secondi dall'avvio compare la scena, o almeno un segnale che sta caricando; il render scale, cioè la risoluzione a cui l'app disegna, non scende sotto l'85%.
I budget di Meta per Quest 3 e 3S: da 1,3 a 1,8 milioni di triangoli per frame. Un **draw call** è una richiesta di disegno che la CPU manda alla GPU: se ne permettono 700–1000 a un'app con poca simulazione, 400–600 con una simulazione media, 200–300 con molta. La simulazione è il lavoro della CPU per fisica, animazioni e rete: più la CPU è occupata lì, meno draw call riesce a mandare.

## Il metodo
Cinque passi, sempre gli stessi:
1. **Misura**, con lo stesso tipo di build e dagli stessi punti.
2. **Chi sfora?** La CPU o la GPU.
3. **Perché?** Trova la causa con lo strumento giusto.
4. **Cambia una cosa sola**: se ne cambi due e il numero scende, non sai quale ha funzionato.
5. **Rimisura** e confronta col numero di prima.
Unity consiglia di misurare prima di cambiare, per avere il punto di partenza, e dopo, per dimostrare che la cura ha funzionato. E di contare i millisecondi, non gli fps: dieci fps in più valgono molto a 30 fps e quasi niente a 90.

## Le regole della misura
- **In editor misuri il PC, non il visore.** Il PC è molto più potente del Quest: i millisecondi dell'editor servono a capire. I conteggi di triangoli e draw call invece dipendono dalla scena e cambiano poco.
- **La build di sviluppo è più lenta.** Serve a collegare il Profiler al visore; i numeri ufficiali vengono dalla build normale.
- **Il primo giro non conta**: al primo avvio gli shader si compilano e la scena scatta.
- **Stessi punti, stessa build, stessa Game view**, se vuoi confrontare un prima e un dopo: con più pixel la GPU lavora di più.

# 1 · Il termometro

## Entra in Play
Premi **Play**. Sei all'ingresso della casa. Clicca nella Game view per guardare col mouse; **Esc** ti ridà il mouse.
> verifica: in-play

## Le statistiche
1. In alto a destra nella Game view premi **Stats**. Sopra la scena compare un riquadro di numeri.
2. Leggi queste righe e scrivi i tuoi numeri:
- **FPS**, i frame al secondo, col tempo di un frame accanto;
- **CPU: main**, il lavoro della CPU sul thread principale, quello degli script;
- **Batches**, i draw call del frame dopo che Unity ne ha uniti alcuni;
- **Tris**, i triangoli disegnati in un frame, ombre comprese;
- **SetPass calls**, quante volte la GPU cambia shader, cioè il programma che colora i pixel;
- **Shadow casters**, gli oggetti che fanno ombra.
Questi numeri li rileggerai qui dopo ogni cura, nelle prossime lezioni.
> lavoro: 2 1

## Chi sfora
Ora scopriamo chi sfora: la CPU, che prepara il frame, o la GPU, che lo disegna.
1. Resta in Play. Dal menu in alto apri **Window ▸ Analysis ▸ Rendering Debugger**. Trascina la sua linguetta sul bordo destro della Game view, finché compare un riquadro, e lascia andare: le due finestre si mettono una accanto all'altra. Non metterla fra le linguette della Game view, perché la Game view finirebbe dietro, e nascosta non disegna.
2. Nella colonna di sinistra clicca **Display Stats**. A destra compare la tabella **Frame Stats**: ogni numero è la media degli ultimi frame, in millisecondi.
3. Leggi e scrivi **Frame Time**, la durata di un frame intero.
4. Leggi e scrivi i due tempi della CPU, **CPU Main Thread Frame** e **CPU Render Thread Frame**, e il tempo della GPU, **GPU Frame**.
Ora confronta.
- Se **Frame Time** è sotto 11,1 ms, sul tuo PC non sfora nessuno.
- Se è sopra, sfora il più lungo dei tre tempi: se è uno dei due della CPU, il demo è CPU-bound; se è **GPU Frame**, è GPU-bound.
- Anche sotto il budget, il più lungo dei tre è il collo di bottiglia: è lui che decide quanto dura il frame.
Se **GPU Frame** resta vicino a zero, il tuo PC non misura la GPU in editor. Nel capitolo 4 lo scopriamo con un esperimento.
> verifica: finestra-aperta Rendering Debugger
> lavoro: 2 1

# 2 · Il Profiler

## Apri il Profiler
1. Dal menu in alto apri **Window ▸ Analysis ▸ Profiler**. Trascina la sua linguetta sul bordo basso della Game view, finché compare un riquadro, e lascia andare: il Profiler si mette sotto la scena, e la Game view resta visibile.
2. In alto a sinistra, nella barra del Profiler, il pulsante rotondo **Record** deve essere acceso. Se è spento, cliccalo.
Il grafico scorre e tiene gli ultimi frame. Ricorda la terza condizione: da adesso i tempi sono più alti del vero. Qui cerchiamo il perché, non il quanto.
> verifica: finestra-aperta Profiler

## Dove va il tempo
1. Nel grafico **CPU Usage** clicca un picco, uno dei frame più alti: il Profiler si ferma su quel frame. Scegli sempre il frame peggiore, perché i problemi stanno lì.
2. Nella metà bassa della finestra, a sinistra, c'è un menu che dice **Timeline**: scegli **Hierarchy**. Compare l'elenco delle funzioni del frame.
3. Clicca l'intestazione della colonna **Time ms**: in cima va quello che costa di più.
4. Apri le righe col triangolino. `PlayerLoop` contiene il frame del gioco, gli script stanno sotto `Update.ScriptRunBehaviourUpdate` e il disegno sotto le righe del render. `EditorLoop` è il lavoro dell'editor: in una build non c'è.
> lavoro: 3 2

## Chi aspetta chi
La CPU lavora su più thread, cioè più linee di lavoro in parallelo. I due che ci interessano sono il **Main Thread**, quello degli script, e il **Render Thread**, che prepara il disegno per la GPU. Quando un thread non ha niente da fare aspetta, e il Profiler scrive anche le attese.
1. Nel menu della metà bassa torna su **Timeline**. Ogni riga è un thread, e ogni blocco colorato è una funzione, largo quanto dura. Ingrandisci con la rotellina del mouse.
2. Nella riga **Main Thread** cerca un blocco lungo con **WaitForGPU** nel nome. Col nostro progetto è `GfxDeviceD3D12.WaitForLastPresentation.WaitForGPU`. Passaci sopra col mouse: compare quanto dura.
3. Nella riga **Render Thread** cerca `GfxDeviceD3D12.WaitForGPU`.
Se i due thread passano buona parte del frame ad aspettare la GPU, la CPU è in anticipo: frena la GPU. Se invece il Render Thread resta a lungo in `Gfx.WaitForGfxCommandsFromMainThread`, sta aspettando il Main Thread: frena la CPU.
Nel demo, cosa vedi?
> lavoro: 4 2

## Highlights
Guarda il proiettore. Il modulo **Highlights** del Profiler segna i frame in cui la CPU o la GPU hanno superato il budget: le tacche rosse sono la CPU, le gialle la GPU. Si accende dal menu **Profiler Modules**, e nel modulo si sceglie 90 fps. I passi sono nella guida.

## Rendering e Memory
1. Dal menu **Profiler Modules** spunta anche **Rendering** e **Memory**.
2. Clicca un frame. In **Rendering** leggi **SetPass Calls**, **Draw Calls**, **Batches** e **Triangles**: gli stessi conteggi delle Stats, frame per frame.
3. In **Memory** guarda quanta memoria occupano le texture. Le texture del demo sono 4K e non compresse: le curiamo nella lezione 9.
Al proiettore ti faccio vedere anche il **Memory Profiler**, uno dei tre strumenti che hai installato. Fa una foto di tutta la memoria e mette in fila gli oggetti, dal più pesante. Lo userai sul tuo progetto, nel laboratorio.
Il modulo **GPU Usage** non lo usiamo: non funziona coi Graphics Jobs del nostro progetto, che dividono il lavoro del disegno su più core della CPU, né sul visore.
> lavoro: 2 1

## Salva la cattura
Guarda il proiettore. Una misura che non salvi non la puoi confrontare. Con **Save**, nella barra del Profiler, salvo i frame registrati in un file `.data`, e con **Load** li riapro. Le catture vanno fuori dal repository, per esempio in Documenti: pesano decine di MB, e `git salva` non le deve mandare.
I professionisti aprono il Profiler in un processo a parte, da **Window ▸ Analysis ▸ Profiler (Standalone Process)**: così la finestra del Profiler non pesa sul frame che misura.

# 3 · Frame Debugger e Rendering Debugger

## Il Frame Debugger
Il Frame Debugger ferma il gioco su un frame e ti mostra, in ordine, tutto quello che la GPU ha disegnato.
1. Dal menu in alto apri **Window ▸ Analysis ▸ Frame Debugger** e premi **Enable**. Il gioco si ferma.
2. A sinistra c'è la lista dei disegni. Clicca una voce: la Game view mostra il frame fino a quel punto.
3. Cerca le voci con **Shadow** nel nome: sono le passate delle ombre.
Nel demo 24 candele sono luci in tempo reale con le ombre. Una luce puntiforme fa ombra in tutte le direzioni, quindi la GPU ridisegna la casa sei volte per ogni candela, a ogni frame. Le curiamo nella lezione 8, col bake, cioè calcolando la luce una volta sola, prima del gioco.
> verifica: finestra-aperta Frame Debugger
> lavoro: 3 2

## Perché non si raggruppa
Guarda il proiettore. Nel Frame Debugger seleziono una voce che disegna gli oggetti della casa: a destra una riga spiega perché quel disegno non si è unito al precedente. Nel demo ogni oggetto ha il suo materiale, e l'SRP Batcher, il sistema di URP che unisce i disegni simili, è spento: ogni oggetto è un draw call a sé. Lo curiamo nella lezione 7.
Poi spegni il tuo Frame Debugger con lo stesso pulsante con cui l'hai acceso: il gioco riparte.

## L'overdraw
L'**overdraw** è lo stesso pixel disegnato più volte nello stesso frame, per esempio dietro un vetro.
1. Nel **Rendering Debugger** clicca **Rendering** nella colonna di sinistra.
2. Metti **Overdraw Mode** su **Transparent**. Un pixel è tanto più chiaro quante più volte è stato disegnato.
3. Guarda le finestre: i vetri sono cinque lastre trasparenti una sull'altra, e ogni pixel dietro il vetro si disegna cinque volte.
4. Rimetti **Overdraw Mode** su **None**.
Sul visore i pixel sono tantissimi, due occhi a piena risoluzione: sono la parte più cara del lavoro della GPU.
> lavoro: 2 1

## La geometria
Guarda il proiettore. Nel Rendering Debugger, in **Rendering**, metto **Additional Wireframe Modes** su **Wireframe** e si vedono i triangoli. I pavimenti e le colonne del demo sono un fitto di linee, troppi triangoli per quello che mostrano. Li curiamo nella lezione 6, coi LOD, versioni più leggere degli oggetti lontani, e col culling, che non disegna quello che non si vede.

## La GPU a tile
La GPU del Quest disegna l'immagine a riquadri, i **tile**, uno alla volta, in una memoria piccola e velocissima dentro il chip, e poi copia ogni tile nella memoria normale, molto più lenta. Per questo sul visore costano tanto le passate che rileggono l'immagine già disegnata: il post-processing, come il bloom, e le copie che URP chiama **Opaque Texture** e **Depth Texture**. Pesa anche l'**HDR**, che usa più byte per pixel. L'**MSAA**, l'antialiasing che leviga i bordi, si fa invece dentro il tile e costa poco.
Nel demo il render scale è 1,5, l'MSAA è spento, l'HDR e il post-processing sono accesi: tutto il contrario. Lo curiamo nella lezione 10.

# 4 · L'esperimento

## Il test della camera
Meta propone due esperimenti per capire chi frena, senza fidarsi di un numero solo. Il primo: spegnere la camera, così non si disegna niente.
1. Nel Profiler spegni **Record**: i tempi tornano veri.
2. Nella casella di ricerca della Hierarchy scrivi `Main Camera` e seleziona la camera del rig del PC.
3. Nell'Inspector togli la spunta accanto a **Camera**. La Game view diventa nera e scrive **No cameras rendering**: nessuna camera disegna.
4. Nel Rendering Debugger, in **Display Stats**, leggi **Frame Time** e scrivilo accanto a quello di prima.
5. Rimetti la spunta a **Camera** e cancella la ricerca.
Se il frame scende di poco, il tempo se lo prende la CPU, con gli script e la fisica. Se crolla, il costo è nel disegno.
> lavoro: 3 2

## Il test dei pixel
Il secondo esperimento: disegnare pochissimi pixel. Meta lo fa abbassando il render scale; noi rimpiccioliamo la Game view, così il demo resta com'è.
1. Nella Game view apri il menu delle proporzioni, di solito **Free Aspect**, e in fondo alla lista premi **+**.
2. Nella finestrella scrivi `Pochi pixel` in **Label**, metti **Type** su **Fixed Resolution**, scrivi `160` e `90` in **Width & Height** e premi **OK**.
3. Scegli **Pochi pixel (160x90)**: la scena diventa a quadrettoni.
4. Col **Record** del Profiler ancora spento, leggi **Frame Time** e **GPU Frame** nel Display Stats e scrivili.
5. Torna al formato di prima, di solito **Free Aspect**.
Guarda soprattutto **GPU Frame**. Se crolla, il lavoro della GPU è nei pixel: overdraw, effetti, risoluzione. Se scende di poco, è nella geometria, cioè nei triangoli da disegnare, anche più volte per le ombre. Se **GPU Frame** crolla ma **Frame Time** scende di poco, adesso frena la CPU, di solito il Render Thread, che prepara i draw call: è il prossimo collo di bottiglia. Se il tuo PC non dà **GPU Frame**, guarda **Frame Time**.
> lavoro: 4 2

## Cosa dicono i due test
Metti insieme i due risultati, come fa Meta:
- camera spenta e il frame scende di poco: frena la CPU, con gli script;
- camera spenta e il frame crolla: il costo è nel disegno, e i pochi pixel dicono dove. Se coi pochi pixel la GPU crolla, il costo è nei pixel; se scende di poco, nella geometria.
Spesso un costo ne nasconde un altro: tolto il lavoro della GPU, il frame si ferma al tempo della CPU.
Nel capitolo 3 il Frame Debugger ti ha già mostrato un sospetto: le ombre delle candele, che ridisegnano la casa tante volte. Adesso lo verifichiamo cambiando una cosa sola.

## Le ombre delle candele
Ora cambia una cosa sola: togli le ombre alle candele. Resta in Play.
1. Nella casella di ricerca della Hierarchy scrivi `Fiamma`: restano le luci delle candele.
2. Clicca la prima, poi tieni **Shift** e clicca l'ultima: le hai selezionate tutte.
3. Nell'Inspector metti **Shadow Type** su **No Shadows**: il campo cambia per tutte insieme.
4. Cancella la ricerca.
> verifica: luci-senza-ombre Fiamma

## Rimisura
1. Col **Record** del Profiler spento, nel Rendering Debugger, in **Display Stats**, rileggi **Frame Time**, **CPU Main Thread Frame**, **CPU Render Thread Frame** e **GPU Frame**.
2. Nelle Stats rileggi **Shadow casters**, **Batches** e **Tris**.
3. Scrivi i numeri di adesso accanto a quelli di prima.
Le ombre erano la causa? Di quanto è sceso il tempo che sforava? Anche senza ombre, però, i triangoli restano sopra il budget di Meta: è la geometria, che curiamo nella lezione 6.
> lavoro: 3 2

## Esci dal Play
Premi **Play** per uscire, e guarda: le ombre delle candele sono tornate. Quello che cambi in Play non resta, e il demo resta il paziente delle prossime lezioni.
> verifica: fuori-play

## La diagnosi del demo
Ecco quello che abbiamo trovato, con lo strumento che lo mostra e la lezione in cui lo curiamo:
- **troppi triangoli**, da Stats, Rendering e Wireframe: pavimenti e colonne troppo densi, niente LOD né culling. Lezione 6;
- **troppi draw call e SetPass**, da Stats, Rendering e Frame Debugger: un materiale per oggetto, l'SRP Batcher spento, i vetri sovrapposti. Lezione 7;
- **le ombre riempiono il frame**, dal Frame Debugger e dalla prova delle candele: candele e sole in tempo reale, con le ombre. Lezione 8;
- **la memoria delle texture**, da Memory e dal Memory Profiler: texture 4K non compresse e senza mipmap, le versioni più piccole che la GPU usa da lontano. Lezione 9;
- **ogni pixel costa troppo**, dall'Overdraw Mode e dalla GPU a tile, e il test dei pixel dice quanto pesano sul tuo PC: render scale 1,5, MSAA spento, HDR, post-processing. Lezione 10.

# 5 · Il visore

## OVR Metrics
Sul visore il termometro è **OVR Metrics Tool**, l'app di Meta che mostra i numeri sopra qualunque app. Guarda il proiettore: te lo faccio vedere in casting. Nel capitolo del progetto lo preparerai sul tuo visore, coi passi completi.

## Leggere OVR Metrics come Meta
- **App GPU Time** è il tempo della GPU per un frame dell'app. Se supera gli 11 111 µs del frame a 90 Hz e gli FPS sono sotto 90, sfora la GPU. Se è sotto ma gli FPS restano bassi, sfora la CPU.
- **GPU Utilization** al 100% vuol dire che la GPU è il collo di bottiglia; già sopra il 90% cominciano i problemi.
- **CPU Utilization** è il core più carico, che può non essere quello del gioco: è l'indizio più debole.
- **CPU level** e **GPU level** sono le frequenze del momento. Il visore le alza e le abbassa da solo, e le tiene basse finché l'app regge; la GPU sale di livello quando è occupata all'87% o più. CPU a 4 e GPU a 2 vuol dire che sfora la CPU, perché la GPU ha ancora margine.
- **Stale Frame Count** conta i frame ripetuti perché quello nuovo non era pronto: è il judder.

## Il demo sul Quest 3S
Guarda il proiettore. Avvio il demo sul visore con OVR Metrics acceso: un giro a vuoto, perché al primo giro gli shader si compilano, poi il giro che conta col benchmark, stazione per stazione. Alla fine i CSV del benchmark e di OVR Metrics passano dal visore al PC. Confronta i numeri del visore con i tuoi: è la stessa scena, su un processore molto meno potente.

## Il Profiler collegato al visore
Guarda il proiettore. I professionisti cercano il perché anche sul dispositivo: una build di sviluppo col Profiler, avviata sul visore col cavo collegato, il comando `adb forward`, e nel Profiler il menu **Play Mode** su **AndroidProfiler(ADB@127.0.0.1:34999)**. Nella Timeline cerchiamo le attese come in editor: i nomi cambiano, il principio è lo stesso. Il tempo della GPU, sul visore, lo dà OVR Metrics. I passi sono nell'ultima pagina del capitolo del progetto.

# Da soli · Verifica

## Il colpevole
Ora tocca a te. Premi il pulsante: si apre la scena `Verifica`, con sei stanze in fila. In ogni stanza le cose si somigliano, ma una costa troppo: è il colpevole. Trovalo con lo strumento giusto. Poi, fuori dal Play, spegnilo: selezionalo e togli la spunta accanto al suo nome, in alto nell'Inspector. Salva con **Ctrl+S**, rientra in Play e rimisura: il numero che saliva deve scendere.
Le regole:
- spegni solo il colpevole: se spegni anche altro, o la stanza intera, il compito non passa;
- misura dalla stazione della stanza, sulla soglia, rivolto dentro, con le condizioni della misura;
- se ti blocchi, sotto ogni compito c'è **Mostra la soluzione**: il compito conta lo stesso, ma resta segnato.
> azione: Apri la verifica | apri-scena | Lezione 05 - Profiling | Verifica

## Stanza 1 · Le statue
Tre statue uguali su tre piedistalli. Davanti a questa stanza **Tris**, nelle Stats, è molto più alto che davanti alle altre. Quale statua?
> verifica: colpevole Statua B | Stanza 1
> lavoro: 5 3
> soluzione: In Play, sulla soglia della stanza 1, apri le Stats: **Tris** è molto più alto che sulla soglia della stanza 2.
> soluzione: Nel Rendering Debugger, in **Rendering**, metti **Additional Wireframe Modes** su **Wireframe**. La `Statua B` è fitta di linee, le altre due no: ha due milioni di triangoli.
> soluzione: Esci dal Play, seleziona `Statua B` e togli la spunta accanto al nome. **Ctrl+S**.

## Stanza 2 · Il mucchio
Due mucchi della stessa grandezza. Davanti a questa stanza salgono **Batches** e **SetPass**. Quale mucchio?
> verifica: colpevole Mucchio B | Stanza 2
> lavoro: 5 3
> soluzione: In Play, sulla soglia della stanza 2, nelle Stats **Batches** è molto più alto che nelle altre stanze.
> soluzione: Apri il **Frame Debugger** e premi **Enable**. Fra le voci degli oggetti opachi ci sono migliaia di disegni, uno per pezzo. Cliccane uno: nella Hierarchy si illumina `Mucchio B`, fatto di duemila pezzi piccoli; `Mucchio A` ne ha venti grandi. Poi spegni il Frame Debugger con lo stesso pulsante.
> soluzione: Esci dal Play, seleziona `Mucchio B` e togli la spunta accanto al nome. **Ctrl+S**.

## Stanza 3 · Il lampadario
Tre lampadari uguali sopra tre tavoli, con sei lampadine ciascuno. Uno fa ombra con tutte e sei. Quale?
> verifica: colpevole Lampadario A | Stanza 3
> lavoro: 5 3
> soluzione: In Play, sulla soglia della stanza 3, apri il **Frame Debugger** e premi **Enable**: le voci delle ombre delle luci puntiformi sono 36, sei per lampadina. Nel Rendering Debugger, in **Rendering**, **Map Overlays** su **Additional Lights Shadow Map** mostra le loro ombre.
> soluzione: Esci dal Play. Nella Hierarchy apri `Lampadario A`, `Lampadario B` e `Lampadario C` e seleziona una lampadina di ognuno: solo quelle di `Lampadario A` hanno **Shadow Type** su **Soft Shadows**.
> soluzione: Seleziona `Lampadario A` e togli la spunta accanto al nome. **Ctrl+S**.

## Stanza 4 · La nebbia
Due veli trasparenti attraversano la stanza. Uno costa alla GPU a ogni pixel che copre. Quale?
> verifica: colpevole Velo B | Stanza 4
> lavoro: 5 3
> soluzione: In Play, sulla soglia della stanza 4: nel Rendering Debugger, in **Rendering**, metti **Overdraw Mode** su **Transparent**. Il velo vicino, `Velo B`, è bianco: sono quaranta strati uno sull'altro. `Velo A`, in fondo, ne ha due.
> soluzione: Fai il test dei pixel del capitolo 4: con **Pochi pixel (160x90)** **GPU Frame** scende molto di più qui che nelle altre stanze. Il costo è nei pixel. Poi torna al formato di prima.
> soluzione: Esci dal Play, seleziona `Velo B` e togli la spunta accanto al nome. **Ctrl+S**.

## Stanza 5 · L'orologio
Tre orologi uguali sul muro. Uno ha uno script che fa lavorare la CPU 4 ms a ogni frame, in tutte le stanze. Quale? Qui il prima e il dopo li confronti con **Profile Analyzer**, lo strumento di Unity che mette a confronto due misure.
1. In Play, sulla soglia della stanza 5, col **Record** del Profiler acceso, resta fermo una decina di secondi.
2. Apri **Window ▸ Analysis ▸ Profile Analyzer**, in alto scegli **Compare** e premi il primo **Pull Data**: il primo grafico, blu, è il tuo prima.
3. Sempre in Play, togli la spunta all'orologio che sospetti. Nel Profiler premi **Clear** e aspetta una decina di secondi.
4. Premi il secondo **Pull Data**: il grafico arancione è il tuo dopo.
5. Nella tabella **Marker Comparison** clicca **Abs Diff**: in cima vanno le righe scese di circa 4 ms. Sono la funzione dell'orologio e le righe che la contengono, come `PlayerLoop`. Se hai preso il colpevole, la sua funzione, `CpuBurner.Update`, nel dopo non c'è più.
Esci dal Play: l'orologio torna acceso. Spegnilo fuori dal Play e salva.
> verifica: colpevole Orologio C | Stanza 5
> lavoro: 8 5
> soluzione: In Play, nel **Profiler**: clicca un frame del grafico **CPU Usage**, scegli **Hierarchy** e ordina per **Time ms**. Apri `PlayerLoop` fino agli script: la riga di `CpuBurner.Update`, che il Profiler scrive `Rufa.Core.dll!Rufa::CpuBurner.Update() [Invoke]`, costa circa 4 ms. In **Timeline** la vedi come un blocco sulla riga **Main Thread**.
> soluzione: Fuori dal Play, nella ricerca della Hierarchy scrivi `t:CpuBurner`: resta `Orologio C`. Con Profile Analyzer, togliendo la spunta a `Orologio C`, `CpuBurner.Update` scende di circa 4 ms come le righe che la contengono, e nel dopo non c'è più.
> soluzione: Seleziona `Orologio C` e togli la spunta accanto al nome. Cancella la ricerca. **Ctrl+S**.

## Extra · Il rubinetto
Due rubinetti uguali. Uno ha uno script che a ogni frame butta via memoria. Ogni tanto il garbage collector, il sistema che libera la memoria non più usata, si mette al lavoro, e quel frame dura molto di più. Quale?
> verifica: colpevole Rubinetto A | Stanza extra
> lavoro: 5 3
> soluzione: In Play, nel **Profiler**, **CPU Usage** in **Hierarchy**: ordina per **GC Alloc**. In cima c'è la riga di `GarbageMaker.Update`, che il Profiler scrive `Rufa.Core.dll!Rufa::GarbageMaker.Update() [Invoke]`: circa 200 KB a frame. Nel grafico si vedono i picchi di `GC.Collect`.
> soluzione: Per vedere chi alloca, nella barra del Profiler accendi **Call Stacks** e clicca un frame nuovo. Seleziona la riga `GC.Alloc` sotto `GarbageMaker.Update` e, nel pannello a destra, scegli **Related Data**.
> soluzione: Esci dal Play. Nella ricerca della Hierarchy scrivi `t:GarbageMaker`: resta `Rubinetto A`. Selezionalo, togli la spunta accanto al nome e cancella la ricerca. **Ctrl+S**.

# Da soli · Il tuo progetto

## La tua scena
Premi il pulsante: si apre la scena del tuo progetto, `Assets/Progetto/Progetto.unity`.
> azione: Apri la tua scena | crea-progetto
> verifica: scena-aperta Progetto

## Le stazioni
Il benchmark misura sempre dagli stessi punti, le stazioni, così il prima e il dopo si confrontano.
1. Premi il pulsante: nella scena compaiono tre stazioni, sotto l'oggetto `Benchmark`.
2. Spostale nei tre punti che contano di più nel tuo blockout, per esempio l'ingresso, il punto più carico e quello dove si vede più lontano.
3. Gira ognuna con la freccia verso quello che si guarda da lì.
4. Salva la scena con **Ctrl+S**.
Dischi e frecce si vedono solo mentre il benchmark misura.
> azione: Aggiungi le stazioni | stazioni-benchmark
> verifica: componente-in-scena Benchmark
> lavoro: 5 3

## Il benchmark sul PC
1. Controlla le condizioni della misura: la Game view davanti, **VSync (Game view only)** spento, il **Record** del Profiler spento.
2. Entra in Play e premi **B**. Il rig salta da una stazione all'altra: a ognuna si ferma 2 secondi e misura per 10.
3. Non toccare niente finché la scritta davanti agli occhi dice **Fatto**.
4. Esci dal Play e premi il pulsante qui sotto: si apre la cartella col CSV, il file dei risultati. Aprilo con Fogli Google, oppure con Excel da **Dati ▸ Da testo/CSV** con l'origine del file su *Stati Uniti*, perché il CSV usa il punto per i decimali.
Il CSV ha una riga per stazione:
- **mean_ms** è il tempo medio di un frame, **fps_mean** gli fps medi;
- **p1_low_ms** è la media dell'1% dei frame peggiori, cioè gli scatti; **max_ms** è il frame peggiore;
- **cpu_mean_ms** è il tempo della CPU, il più lungo fra Main Thread e Render Thread; **gpu_mean_ms** è il tempo della GPU.
Se **mean_ms** resta sotto 11,1 ms, non sfora nessuno. Se lo supera, sfora il più lungo fra **cpu_mean_ms** e **gpu_mean_ms**. Se **gpu_mean_ms** è 0, quel PC non dà il tempo della GPU: decide il test della camera. La prima riga del file dice com'è stata presa la misura: PC o VR, build di sviluppo o no, render scale, MSAA.
Non è ancora il numero del visore: è il tuo PC.
> azione: Apri la cartella dei CSV | apri-cartella-benchmark
> attesa: 40 Il giro del benchmark
> lavoro: 3 2

## La build per il visore
1. Seleziona l'oggetto `Benchmark` e nell'Inspector accendi **Start On Launch**: sul visore il benchmark parte da solo all'avvio.
2. Salva la scena con **Ctrl+S**: la build prende la scena salvata.
3. Dal menu in alto scegli **RUFA ▸ Build ▸ Quest (APK)** e aspetta la fine della build.
4. Collega il visore col cavo e installa l'app come nella lezione 1: nel terminale, dalla cartella `sandbox`, `adb install -r "Builds/Quest/RUFA_Sandbox.apk"`.
> attesa: 300 La build
> lavoro: 5 3

## OVR Metrics sul tuo visore
Prepara OVR Metrics prima di avviare la tua app: se lo installi mentre l'app gira, il sistema la chiude.
1. Nel visore apri lo **Store**, cerca `OVR Metrics Tool` e installalo, se non c'è già.
2. Aprilo dalla **Libreria**.
3. Accendi **Enable Persistent Overlay**: i numeri restano davanti agli occhi in ogni app.
4. Scegli il preset **Basic**. Non **Advanced**: Meta avverte che pesa e falsa i numeri.
5. Accendi **Record all captured metrics to CSV file**: da ora ogni avvio di un'app scrive un CSV sul visore.
> lavoro: 5 3

## Al visore
1. Avvia la tua app dalla **Libreria**, fra le origini sconosciute.
2. Fai un giro a vuoto delle stazioni: al primo giro gli shader si compilano.
3. Chiudi l'app e riavviala. Ora il giro che conta: vai sul disco di ogni stazione e guarda la freccia, finché la scritta dice che la misura è fatta.
4. Alla stazione più pesante leggi l'overlay e scrivi su un foglio **Average FPS**, **App GPU Time**, **CPU Utilization**, **GPU Utilization**, **CPU level** e **GPU level**.
> lavoro: 10 6

## Le misure
1. Collega il visore al PC col cavo e premi il pulsante qui sotto.
2. I CSV del benchmark arrivano in `Consegne/L05/`, quelli di OVR Metrics in `Consegne/L05/OVR Metrics/`.
Il pulsante copia solo quello che il visore ha scritto dopo la tua build, perché sul visore ci sono anche le misure di chi è venuto prima. Il file più recente è il giro che conta: il suo nome va nel report. Se la Console dice che non trova il visore, controlla il cavo e che nel visore sia permesso il debug USB.
> azione: Scarica le misure dal visore | scarica-misure | Consegne/L05
> verifica: file-esiste Consegne/L05/benchmark-*.csv | Consegne/L05/OVR Metrics/*.csv

## Start On Launch spento
1. Seleziona `Benchmark` e spegni **Start On Launch**.
2. Salva la scena con **Ctrl+S**: la prossima build non deve partire misurando.
> verifica: scena-vero Benchmark | Benchmark | startOnLaunch | no

## Il perché
Il visore ti ha detto quanto costa e chi sfora. Il perché lo cerchi in editor, alla stazione più pesante, con gli strumenti di oggi:
1. **Chi sfora**, con le condizioni della misura: Display Stats.
2. **Chi aspetta chi**: la Timeline del Profiler.
3. **I due test di Meta**: la camera spenta e i pochi pixel.
4. **Cosa si disegna**: Stats, Frame Debugger, Overdraw Mode e Wireframe.
Trova le tre cause principali, e per ognuna lo strumento che la mostra. Scrivile: vanno nel report.
> lavoro: 15 10

## Project Auditor
Project Auditor legge tutto il progetto senza entrare in Play, e segnala quello che può costare: impostazioni, asset e script.
1. Dal menu in alto apri **Window ▸ Analysis ▸ Project Auditor**.
2. Premi **Start Analysis** e aspetta la fine: ci vuole qualche minuto.
3. Compare il riepilogo, **Summary**. Passa alla vista **Assets** e scegli **Asset Issues**: è la tabella delle segnalazioni sugli asset, con le colonne **Issue**, **Severity**, **Areas** e **Path**.
4. Clicca l'intestazione **Path**: le segnalazioni si ordinano per cartella, e quelle di `Assets/Progetto` stanno insieme. **Severity** dice quanto è grave ognuna.
5. Guarda anche la vista **Project**: sono le segnalazioni sulle impostazioni, che valgono per tutto il progetto.
6. Scrivi le tre più gravi che riguardano il tuo progetto: vanno nel report.
Molte segnalazioni riguardano il demo e le palestre, che hanno i difetti apposta: guarda solo il tuo progetto.
> verifica: finestra-aperta Project Auditor
> lavoro: 10 6

## Memory Profiler
Il Memory Profiler fa una foto della memoria e mette in fila gli oggetti, dal più pesante.
1. Entra in Play nella tua scena.
2. Dal menu in alto apri **Window ▸ Analysis ▸ Memory Profiler**.
3. In alto premi **Capture** e aspetta: la foto compare nell'elenco a sinistra.
4. Selezionala e apri la scheda **Unity Objects**. Clicca l'intestazione **Allocated Size**: in cima vanno gli oggetti più pesanti.
5. Scrivi i tre oggetti del tuo progetto che pesano di più, col loro tipo, per esempio texture, mesh o audio. Vanno nel report.
6. Esci dal Play.
La foto contiene anche la memoria dell'editor, quindi i totali sono più alti di quelli del visore: qui conta l'ordine. Le foto finiscono nella cartella `MemoryCaptures` del progetto, che git ignora.
> verifica: file-esiste sandbox/MemoryCaptures/*.snap
> lavoro: 8 5

## Il report
Premi il pulsante: crea `Consegne/L05/report.md` dal modello e lo apre. Se Windows chiede con quale programma, scegli Blocco note. Al posto di ogni «da scrivere» metti il tuo testo: la build, le condizioni della misura, le stazioni coi numeri del CSV, i numeri di OVR Metrics, chi sfora e come lo sai, i due test di Meta, Project Auditor, il Memory Profiler, le tre cause, cosa curerai e in quale lezione.
> azione: Crea il report | crea-report | Consegne/L05/report.md
> verifica: file-compilato Consegne/L05/report.md | «da scrivere»
> lavoro: 15 10

## Extra · Il Profiler sul visore
Per chi vuole il perché sul dispositivo, come i professionisti:
1. **RUFA ▸ Build ▸ Quest (Development, Profiler)**, poi, dalla cartella `sandbox`, `adb install -r "Builds/Quest/RUFA_Sandbox_dev.apk"`.
2. Avvia l'app sul visore, col cavo collegato.
3. Nel terminale: `adb forward tcp:34999 localabstract:Unity-it.rufa.gameart3.sandbox`.
4. Nel Profiler, nel menu in alto a sinistra che dice **Play Mode**, scegli **AndroidProfiler(ADB@127.0.0.1:34999)**.
5. Alla stazione più pesante prendi il frame peggiore e guardalo in **Timeline** e in **Hierarchy**. Cerca le attese come in editor.
6. Premi **Save** e chiama il file `visore.data`, nella cartella **Documenti**, fuori dal repository.
Poi reinstalla la build normale: i numeri del report vengono da quella.

## A fine lezione
**Ctrl+S**, **File ▸ Save Project**, chiudi Unity e lancia `git salva "Lezione 05"`: il report e i CSV vanno sul tuo repository. Le catture del Profiler restano in Documenti, e le foto della memoria in `MemoryCaptures`, fuori da git.
