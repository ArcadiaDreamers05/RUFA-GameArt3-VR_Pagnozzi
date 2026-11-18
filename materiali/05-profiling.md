# Lezione 5 — Profiling e frame budget: la guida

Questa è la guida da tenere aperta mentre misuri. Ci sono le stesse procedure del tutorial, tutte in un posto solo e in ordine, più quello che in aula ti ho fatto vedere al proiettore.

## 1. Prima di misurare: le tre condizioni
Una misura in editor vale solo a tre condizioni.
1. **La Game view sta davanti.** Se è nella stessa finestra della Scene view, clicca la linguetta **Game**. Una Game view nascosta non disegna il gioco, e i tempi diventano quelli di una scena vuota.
2. **Il VSync della Game view è spento.** In alto nella Game view apri il menu delle proporzioni, quello con scritto **Free Aspect**, e togli la spunta a **VSync (Game view only)**. Con la spunta il frame aspetta il monitor: su uno schermo a 60 Hz non scende mai sotto i 16,7 ms.
3. **Il Profiler non registra.** Mentre registra, il frame dura molto di più. Leggi i tempi col pulsante **Record** del Profiler spento: il Profiler serve per il perché.

Per confrontare un prima e un dopo, tieni anche la Game view della stessa grandezza: con più pixel la GPU lavora di più.

## 2. Chi sfora: la CPU o la GPU
La CPU prepara il frame, la GPU lo disegna. Lavorano in parallelo, e ognuna ha tutto il frame: a 90 Hz, 11,1 ms.
1. In Play apri **Window ▸ Analysis ▸ Rendering Debugger** e, nella colonna di sinistra, clicca **Display Stats**.
2. Nella tabella **Frame Stats** leggi **Frame Time**, **CPU Main Thread Frame**, **CPU Render Thread Frame** e **GPU Frame**.
3. Se **Frame Time** è sotto 11,1 ms, sul tuo PC non sfora nessuno. Se è sopra, sfora il più lungo dei tre tempi: uno dei due della CPU, e allora il gioco è CPU-bound, oppure **GPU Frame**, e allora è GPU-bound.
4. Se **GPU Frame** resta vicino a zero, il tuo PC non misura la GPU in editor: decide il test della camera, nella sezione 4.

Anche sotto il budget, il più lungo dei tre è il collo di bottiglia: è lui che decide quanto dura il frame.

Le **Stats** della Game view danno invece i conteggi: **Batches**, cioè i draw call dopo che Unity ne ha uniti alcuni, **Tris**, **SetPass calls** e **Shadow casters**.

## 3. Chi aspetta chi: la Timeline del Profiler
Quando un thread non ha niente da fare aspetta, e il Profiler scrive anche le attese. Sono la prova più diretta di chi frena.
1. Apri **Window ▸ Analysis ▸ Profiler**, accendi **Record** ed entra in Play.
2. Clicca un picco del grafico **CPU Usage**. Nel menu della metà bassa della finestra scegli **Timeline**.
3. Nelle righe **Main Thread** e **Render Thread** cerca i blocchi lunghi con **Wait** nel nome. Ingrandisci con la rotellina; passa sopra un blocco col mouse per leggere quanto dura.

Come si leggono:
- **La CPU aspetta la GPU: frena la GPU.** Sul PC, con DirectX 12, i blocchi sono `GfxDeviceD3D12.WaitForLastPresentation.WaitForGPU` sul Main Thread e `GfxDeviceD3D12.WaitForGPU` sul Render Thread. Su altre piattaforme il manuale di Unity cita `Gfx.WaitForPresentOnGfxThread` sul Main Thread e `Gfx.PresentFrame` sul Render Thread.
- **Il Render Thread aspetta il Main Thread: frena la CPU.** Il blocco è `Gfx.WaitForGfxCommandsFromMainThread`.
- **Il frame aspetta lo schermo: non frena nessuno.** Il blocco è `WaitForTargetFPS`, e compare col VSync acceso o con un frame rate fissato.
- **`EditorLoop` è il lavoro dell'editor**: in una build non c'è.

Sul visore i nomi cambiano, ma il principio è lo stesso: cerca chi aspetta.

## 4. I due test di Meta
Vengono dalla guida di Meta *Basic Optimization Workflow for Meta Quest Apps*. Sono esperimenti, quindi valgono su qualunque PC, anche dove la GPU non si misura. Leggi **Frame Time** prima e dopo, col Profiler fermo.

**Il test della camera.** In Play, nella casella di ricerca della Hierarchy, scrivi `Main Camera` e seleziona la camera del rig del PC. Nell'Inspector togli la spunta accanto al componente **Camera**: non si disegna più niente, e la Game view diventa nera e scrive **No cameras rendering**. Se il frame scende di poco, frena la CPU, con gli script e la fisica. Se crolla, il costo è nel disegno. Poi rimetti la spunta.

**Il test dei pixel.**
1. Nella Game view apri il menu delle proporzioni, di solito **Free Aspect**, e in fondo alla lista premi **+**.
2. Nella finestrella scrivi `Pochi pixel` in **Label**, metti **Type** su **Fixed Resolution**, scrivi `160` e `90` in **Width & Height** e premi **OK**.
3. Scegli **Pochi pixel (160x90)** e leggi **Frame Time** e **GPU Frame**.
4. Torna al formato di prima.

Guarda soprattutto **GPU Frame**. Se crolla, il lavoro della GPU è nei pixel, cioè overdraw, effetti, risoluzione; se scende di poco, è nella geometria, cioè i triangoli, anche disegnati più volte per le ombre. Se **GPU Frame** crolla ma **Frame Time** scende di poco, adesso frena la CPU, di solito il Render Thread, che prepara i draw call: è il prossimo collo di bottiglia. Se il tuo PC non dà **GPU Frame**, guarda **Frame Time**.

Meta fa lo stesso test abbassando il render scale a un valore minuscolo; noi rimpiccioliamo la Game view, così l'asset del progetto non cambia.

Come si mettono insieme:
- camera spenta e il frame scende di poco: frena la CPU, con gli script e la fisica;
- camera spenta e il frame crolla: il costo è nel disegno, e i pochi pixel dicono dove. Se coi pochi pixel la GPU crolla, il costo è nei pixel; se scende di poco, nella geometria.

Spesso un costo ne nasconde un altro: tolto il lavoro della GPU, il frame si ferma al tempo della CPU.

Meta aggiunge due regole. Uno script che prende più di 2 ms si può quasi sempre ottimizzare. E dopo ogni cura si rifà tutto il giro, perché chi frena può cambiare.

## 5. Il Profiler come i professionisti
- **Il frame peggiore.** Clicca un picco del grafico, non un frame qualsiasi.
- **Cosa costa.** Nella metà bassa scegli **Hierarchy** e clicca **Time ms**: in cima va quello che costa di più. `PlayerLoop` contiene il frame del gioco, e gli script stanno sotto `Update.ScriptRunBehaviourUpdate`.
- **La memoria buttata.** La colonna **GC Alloc** mostra la memoria allocata a ogni frame. Per sapere chi la alloca, nella barra del Profiler accendi **Call Stacks**, clicca un frame nuovo, seleziona la riga `GC.Alloc` e scegli **Related Data** nel pannello di destra.
- **Highlights.** Dal menu **Profiler Modules** spunta **Highlights** e scegli 90 fps: le tacche rosse sono i frame in cui ha sforato la CPU, le gialle quelli in cui ha sforato la GPU.
- **Rendering e Memory.** Dallo stesso menu: in **Rendering** i conteggi del frame, **SetPass Calls**, **Draw Calls**, **Batches** e **Triangles**; in **Memory** la memoria delle texture.
- **Il processo a parte.** **Window ▸ Analysis ▸ Profiler (Standalone Process)**: così la finestra del Profiler non pesa sul frame che misura.
- **Le catture.** **Save** salva i frame in un file `.data`, **Load** lo riapre. Salvale in Documenti, fuori dal repository: pesano decine di MB, e `git salva` non le deve mandare.
- **Deep Profile** solo quando la Hierarchy non basta: misura ogni funzione e rallenta tutto.
- **GPU Usage** non lo usiamo: non funziona coi Graphics Jobs del nostro progetto, che dividono il lavoro del disegno su più core della CPU, né sul visore, che usa Vulkan, il sistema grafico del Quest.
- **Sul visore.** **RUFA ▸ Build ▸ Quest (Development, Profiler)**, poi, dalla cartella `sandbox`, `adb install -r "Builds/Quest/RUFA_Sandbox_dev.apk"`. Avvia l'app col cavo collegato e lancia `adb forward tcp:34999 localabstract:Unity-it.rufa.gameart3.sandbox`. Nel Profiler, nel menu in alto a sinistra che dice **Play Mode**, scegli **AndroidProfiler(ADB@127.0.0.1:34999)**. In alternativa, da **File ▸ Build Profiles** accendi **Development Build** e **Autoconnect Profiler** e premi **Build And Run**. La build di sviluppo è più lenta: serve per il perché, i numeri ufficiali vengono dalla build normale. Il tempo della GPU, sul visore, lo dà OVR Metrics.

## 6. Il prima e il dopo con Profile Analyzer
Profile Analyzer è il package di Unity che confronta due misure del Profiler. Si installa da **Window ▸ Package Manager**: a sinistra **Unity Registry**, nella ricerca `Profile Analyzer`, poi **Install**.
1. **Il prima.** In Play, col **Record** del Profiler acceso, resta fermo una decina di secondi. Apri **Window ▸ Analysis ▸ Profile Analyzer**, in alto scegli **Compare** e premi il primo **Pull Data**: il primo grafico, blu, si riempie.
2. **Cambia una cosa sola.**
3. **Il dopo.** Nel Profiler premi **Clear**, aspetta una decina di secondi, poi in Profile Analyzer premi il secondo **Pull Data**: il secondo grafico, arancione, si riempie.
4. **Il confronto.** Leggi la durata mediana del frame prima e dopo. Nella tabella **Marker Comparison** clicca **Abs Diff**: in cima vanno le funzioni cambiate di più.

La **mediana** è il valore di mezzo: metà dei frame dura di più, metà di meno, e un frame strano non la sposta. Le due misure sono gonfiate dal Profiler nello stesso modo, quindi il confronto è giusto. Per confrontare una cattura salvata, aprila prima con **Load** nel Profiler, poi premi **Pull Data**.

## 7. La foto della memoria con Memory Profiler
1. Installa **Memory Profiler** dal Package Manager, come sopra.
2. In Play apri **Window ▸ Analysis ▸ Memory Profiler** e premi **Capture**.
3. Seleziona la foto nell'elenco a sinistra, apri la scheda **Unity Objects** e clicca **Allocated Size**: in cima vanno gli oggetti più pesanti.

La foto fatta in editor contiene anche la memoria dell'editor: i totali sono più alti di quelli del visore, ma l'ordine degli oggetti è utile. La misura vera si fa su una build di sviluppo del visore: prima di premere **Capture**, sceglila nel menu accanto, quello che di solito dice **Play Mode**. Meta dà a un'app del Quest 3 e del 3S al massimo 5,75 GiB di memoria. Le foto vanno nella cartella `MemoryCaptures` del progetto, che git ignora. La memoria la curiamo nella lezione 9.

## 8. Il controllo del progetto con Project Auditor
1. Installa **Project Auditor** dal Package Manager, come sopra.
2. Apri **Window ▸ Analysis ▸ Project Auditor** e premi **Start Analysis**. Legge asset, impostazioni e script senza entrare in Play, e ci vuole qualche minuto.
3. Dal riepilogo, **Summary**, passa alla vista **Assets** e scegli **Asset Issues**: la tabella ha le colonne **Issue**, **Severity**, **Areas** e **Path**.
4. Clicca **Path** per ordinare per cartella, e tieni solo le segnalazioni della tua cartella, `Assets/Progetto`: quelle del demo e delle palestre sono difetti messi apposta. **Severity** dice quanto è grave ognuna.
5. Guarda anche le viste **Code**, per gli script, e **Project**, per le impostazioni, che valgono per tutto il progetto.

Unity consiglia di farlo girare nei momenti importanti del lavoro, per esempio prima di ogni consegna.

## 9. Attivare OVR Metrics e leggerlo
Sul visore, prima di avviare l'app da misurare:
1. Nello **Store** del visore cerca `OVR Metrics Tool` e installalo. Se lo installi mentre l'app da misurare gira, il sistema la chiude.
2. Aprilo dalla **Libreria**.
3. Accendi **Enable Persistent Overlay**: i numeri restano sopra ogni app.
4. Scegli il preset **Basic**: Average FPS, App GPU Time, CPU e GPU Utilization, CPU e GPU level, Stale Frame Count e la batteria. Non **Advanced**: Meta avverte che pesa e falsa i numeri.
5. Per registrare, accendi **Record all captured metrics to CSV file**. Ogni avvio di un'app scrive un CSV in `/sdcard/Android/data/com.oculus.ovrmonitormetricsservice/files/CapturedMetrics/`.
6. Avvia l'app. L'overlay va circa un secondo indietro rispetto a quello che vedi.

Come si legge, con le regole di Meta:
- **App GPU Time** è il tempo della GPU per un frame, in microsecondi. Oltre gli 11 111 µs del frame a 90 Hz, con gli FPS sotto 90, sfora la GPU. Sotto il frame ma con gli FPS bassi, sfora la CPU.
- **GPU Utilization** al 100% vuol dire che la GPU è il collo di bottiglia; già sopra il 90% cominciano i problemi.
- **CPU Utilization** è il core più carico, che può non essere quello del gioco: è l'indizio più debole.
- **CPU level** e **GPU level** sono spiegati nella sezione 13.
- **Stale Frame Count** conta i frame ripetuti perché quello nuovo non era pronto: è il judder.

## 10. Raccogliere i dati
Il protocollo, ogni volta uguale:
1. Build normale, non di sviluppo.
2. OVR Metrics pronto prima di avviare l'app: overlay, preset **Basic**, registrazione accesa.
3. Un giro a vuoto: al primo giro gli shader si compilano. Meta consiglia di fare ogni pezzo due volte e di tenere il secondo giro.
4. Il giro che conta, col benchmark, dalle stesse stazioni. Alla stazione più pesante leggi l'overlay e scrivi i numeri.
5. Col cavo collegato, nel tutorial premi **Scarica le misure dal visore**: i CSV del benchmark vanno in `Consegne/L05/`, quelli di OVR Metrics in `Consegne/L05/OVR Metrics/`. Il pulsante copia solo quelli scritti dopo la tua build, perché il visore tiene anche le misure di chi è venuto prima; il più recente è il giro che conta. Senza il tutorial: `adb pull /sdcard/Android/data/it.rufa.gameart3.sandbox/files/ .` e `adb pull /sdcard/Android/data/com.oculus.ovrmonitormetricsservice/files/CapturedMetrics/ .`
6. Fra il prima e il dopo cambi una cosa sola, e scrivi quale.

## 11. Il benchmark e il CSV
- **Le stazioni.** Nel tutorial premi **Aggiungi le stazioni**, oppure scegli **RUFA ▸ Setup ▸ 5 - Crea stazioni di benchmark**. Spostale nei punti che contano, con la freccia verso quello che si guarda.
- **Sul PC.** Con le condizioni della sezione 1 e il Simulatore VR spento, in Play premi **B**: il rig va da una stazione all'altra, 2 secondi di attesa e 10 di misura ciascuna. Il CSV va in `%USERPROFILE%\AppData\LocalLow\RUFA\RUFA Sandbox\`.
- **Sul visore.** Accendi **Start On Launch** sul componente `Benchmark`, fai la build normale, poi vai sul disco di ogni stazione e guarda la freccia. Dopo, spegni **Start On Launch**.
- **Le colonne.**
  - `mean_ms` è il frame medio, `fps_mean` gli fps medi.
  - `p1_low_ms` è la media dell'1% dei frame peggiori, cioè gli scatti; `max_ms` è il frame peggiore.
  - `cpu_mean_ms` è il tempo della CPU, il più lungo fra Main Thread e Render Thread.
  - `gpu_mean_ms` è il tempo della GPU.
- **Come si legge.** Se `mean_ms` resta entro 11,1 ms non sfora nessuno. Se lo supera, sfora il più lungo fra `cpu_mean_ms` e `gpu_mean_ms`. Se `gpu_mean_ms` è 0, quel PC o quel visore non danno il tempo della GPU all'app: in editor decide il test della camera, sul visore App GPU Time di OVR Metrics. La prima riga del file dice modo, build, render scale, MSAA e da dove viene il tempo della GPU.
- **Aprirlo.** Usa Fogli Google, oppure Excel da **Dati ▸ Da testo/CSV** con l'origine *Stati Uniti*, perché il CSV usa il punto per i decimali.

## 12. Il frame in VR e le regole dello Store
Lo schermo del Quest 3S si aggiorna a frequenza fissa, 72, 90 o 120 volte al secondo: per noi 90, cioè un frame ogni 11,1 ms. Se il frame nuovo non è pronto, il visore ripete il precedente, spostato per seguire la testa. L'immagine segue ancora lo sguardo, ma le cose che si muovono nella scena scattano: è il judder, e OVR Metrics lo conta come stale frame. C'è anche un modo previsto di andare a metà velocità, **Application SpaceWarp**: l'app disegna un frame sì e uno no, e il visore ricostruisce quelli in mezzo. Noi non lo usiamo.

Le regole dello Store di Meta, che si chiamano VRC:
- **VRC.Quest.Performance.1**, obbligatoria. L'app deve fare almeno 60 fps, e un'app interattiva deve usare lo schermo a 72, 80, 90, 96, 100 o 120 Hz. Meta la controlla usando l'app per tutta la sua durata, o per 45 minuti se dura di più, e guardando il grafico degli FPS di OVR Metrics.
- **VRC.Quest.Performance.3**, obbligatoria: entro 4 secondi dall'avvio, la grafica che segue la testa oppure un indicatore di caricamento nel visore.
- **VRC.Quest.Performance.4**, consigliata: il render scale non sotto l'85%.

I budget di Meta per Quest 3 e 3S: da 1,3 a 1,8 milioni di triangoli per frame; draw call 700–1000 per un'app con poca simulazione, 400–600 con una simulazione media, 200–300 con molta. La simulazione è il lavoro della CPU per fisica, animazioni e rete: più la CPU è occupata lì, meno draw call riesce a mandare.

## 13. I livelli di CPU e GPU
Il Quest non va sempre alla massima velocità. Il sistema sceglie per la CPU e per la GPU un livello di frequenza, il più basso con cui l'app regge la frequenza dello schermo: così il visore consuma e scalda meno. La GPU sale di livello quando è occupata all'87% o più, e scende sotto l'81%.

In OVR Metrics, **CPU level** e **GPU level** mostrano i livelli del momento. CPU a 4 e GPU a 2 vuol dire che sfora la CPU, perché la GPU ha ancora margine. Se sono tutti e due al massimo, guarda le altre metriche.

Per confrontare un prima e un dopo, Meta consiglia di bloccare i livelli, così il sistema non li cambia fra una misura e l'altra. Lo faremo dalla lezione 6: col visore collegato, `adb shell setprop debug.oculus.cpuLevel 4` e `adb shell setprop debug.oculus.gpuLevel 4`. Il blocco si perde quando il visore si riavvia.

## 14. La GPU a tile
Tutti i visori Meta Quest disegnano a riquadri, i **tile**. La GPU divide l'immagine in tile, li disegna uno alla volta in una memoria piccola e velocissima dentro il chip, e poi copia ogni tile finito nella memoria normale. Secondo Meta, leggere dalla memoria normale è circa dieci volte più lento che leggere dal tile.

Cosa vuol dire quando misuri:
- costano molto le passate che rileggono l'immagine già disegnata: il post-processing, come bloom e blur, e le copie che URP chiama **Opaque Texture** e **Depth Texture**. Ognuna costringe a copiare tutta l'immagine nella memoria lenta e a rileggerla;
- l'**HDR** usa più byte per pixel, quindi c'è più memoria da muovere;
- l'**MSAA**, l'antialiasing che leviga i bordi, si fa dentro il tile e sul Quest costa poco: Meta lo misura molto economico a 4×, Unity consiglia 2× come buon compromesso;
- da Unity 6.3, URP può fare una parte del post-processing dentro il tile.

Nel demo il render scale è 1,5, l'MSAA è spento, l'HDR e il post-processing sono accesi: lo curiamo nella lezione 10.

## 15. Il report, cioè la consegna
Nel tutorial premi **Crea il report**: nasce `Consegne/L05/report.md`. Al posto di ogni «da scrivere» metti:
- piattaforma e build;
- le condizioni della misura;
- le stazioni coi numeri del CSV, del PC e del visore;
- i numeri di OVR Metrics alla stazione più pesante;
- chi sfora e come lo sai: i tempi, chi aspetta chi, App GPU Time;
- i due test di Meta sul tuo blockout;
- le tre segnalazioni più gravi di Project Auditor;
- i tre oggetti più pesanti del Memory Profiler;
- tre cause, con lo strumento che le mostra, e cosa curerai e in quale lezione.

Accanto ci sono i CSV del benchmark e di OVR Metrics. Poi, a Unity chiuso, `git salva "Lezione 05"`.

## 16. Problemi comuni
| Sintomo | Causa | Rimedio |
|---|---|---|
| Nessun frame scende sotto i 16,7 ms | il VSync della Game view è acceso, su uno schermo a 60 Hz | togli la spunta a **VSync (Game view only)** |
| I tempi sono quasi zero anche nel demo | la Game view è nascosta dietro la Scene view, o dietro un'altra finestra nella stessa scheda | clicca la linguetta **Game**; le altre finestre mettile sul bordo della Game view, non fra le sue linguette |
| I tempi sono alti e saltano | il Profiler sta registrando | spegni **Record** e rileggi |
| **GPU Frame** e `gpu_mean_ms` restano a zero | il tuo PC non dà il tempo della GPU all'editor | il test della camera; sul visore, App GPU Time |
| `gpu_mean_ms` è 0 nel CSV del visore | il visore non dà il tempo della GPU all'app | App GPU Time di OVR Metrics |
| Un package non si installa | Unity Hub è chiuso o non connesso | apri Unity Hub, entra col tuo account, riprova |
| Col tasto B il rig non si sposta | il Simulatore VR è acceso | spegnilo da **RUFA ▸ Play mode ▸ Simulatore VR** |
| **Pull Data** non porta niente | il Profiler era vuoto | **Record** acceso, aspetta una decina di secondi, riprova |
| **Scarica le misure** dice che non trova il visore | il cavo, o il debug USB non permesso | ricollega il cavo e accetta il debug USB nel visore |
| In OVR Metrics non c'è il CSV | la registrazione è stata accesa dopo l'avvio dell'app | accendi **Record all captured metrics to CSV file**, poi riavvia l'app |
| In Excel i numeri sono sbagliati | il punto decimale letto all'italiana | origine *Stati Uniti*, o Fogli Google |
| Il Profiler non vede il visore | build normale, o `adb forward` con l'identificativo sbagliato | build di sviluppo, `localabstract:Unity-it.rufa.gameart3.sandbox` |
| GPU Usage dice che non è supportato | i Graphics Jobs, o Vulkan sul visore | i tempi del Display Stats in editor, OVR Metrics sul visore |

## 17. Per saperne di più
Unity:
- *Ultimate guide to profiling Unity games (Unity 6 edition)*: https://unity.com/resources/ultimate-guide-to-profiling-unity-games-unity-6
- I marker del Profiler: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/profiler/markers
- Il modulo Highlights: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/profiler/visualizing-data/highlights
- Profile Analyzer: https://docs.unity3d.com/Packages/com.unity.performance.profile-analyzer@1.4/manual/index.html
- Memory Profiler: https://docs.unity3d.com/Packages/com.unity.memoryprofiler@1.1/manual/index.html
- Project Auditor: https://docs.unity3d.com/Packages/com.unity.project-auditor@3.1/manual/index.html
- URP sui visori senza cavo: https://docs.unity.com/en-us/engine/6000.3/manual/xr/graphics/untethered-device-optimization

Meta:
- Basic Optimization Workflow for Meta Quest Apps: https://developers.meta.com/horizon/documentation/unity/po-perf-opt-mobile/
- Testing and performance analysis: https://developers.meta.com/horizon/documentation/unity/unity-perf/
- OVR Metrics Tool: https://developers.meta.com/horizon/documentation/unity/ts-ovrmetricstool/
- I livelli di CPU e GPU: https://developers.meta.com/horizon/documentation/unity/os-cpu-gpu-levels/
- La GPU a tile: https://developers.meta.com/horizon/documentation/unity/gpu-tiled/
- Gli effetti che sui tile costano: https://developers.meta.com/horizon/documentation/unity/gpu-impaired-algorithms/
- VRC.Quest.Performance.1: https://developers.meta.com/horizon/resources/vrc-quest-performance-1/

## 18. Gli strumenti di Meta per la GPU
Per chi vuole andare oltre, Meta ha altri strumenti, tutti da usare col visore collegato:
- **ovrgpuprofiler** è già sul visore e si lancia con `adb shell ovrgpuprofiler`: legge i contatori della GPU e dice se lavora soprattutto sui vertici, sui pixel o sulle texture;
- **Meta Quest Developer Hub** è l'app di Meta per il PC: mostra in grafico le misure di OVR Metrics e registra le tracce di sistema con **Perfetto**, dove si vede frame per frame chi è arrivato tardi;
- **RenderDoc Meta Fork** cattura un frame sul visore e mostra il costo di ogni disegno e dei tile. Lo useremo nella lezione 10.
