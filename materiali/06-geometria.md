# Lezione 6 — La geometria nel museo: la guida

Questa è la guida da tenere aperta mentre lavori. Ci sono le stesse procedure del tutorial, tutte in un posto solo e in ordine, più le regole di Meta e di Unity con le fonti.

## 1. Il cliente e il budget
Un **triangolo** è il pezzo più piccolo di un modello 3D. Il visore deve trasformare ogni triangolo, cioè calcolare dove cade sullo schermo, e poi colorarlo; in VR lo fa per due occhi.

Meta indica quanti triangoli può disegnare il Quest 3 e il 3S in un frame: **da 1,3 a 1,8 milioni**. È un totale per frame, non per oggetto: dentro ci deve stare tutto, pareti, opere, personaggi, effetti e interfaccia. Per i draw call, cioè le richieste di disegno che la CPU manda alla GPU, Meta indica 200–300 se l'app ha molta simulazione, 400–600 con una media, 700–1000 con poca: li curiamo nella lezione 7.

Il budget di solito non lo decidi tu: lo dà il cliente o lo studio prima di cominciare. Nel museo il cliente l'ha scritto su un cartello all'ingresso di ogni sala.

## 2. Come si misura
Nel museo ogni parte ha una camera fissa all'ingresso. Si misura fuori dal Play.
1. Clicca la linguetta **Game**.
2. In alto a sinistra nella Game view apri il menu **Display 1** e scegli il display della sala: **Display 2** il salone, **3** il giardino, **5** le curiosità. Il deposito ha una scena sua, `Deposito`, e la sua camera è sul **Display 4**.
3. Nel menu delle proporzioni scegli **16:9 Aspect**, così l'inquadratura è uguale su tutti i PC.
4. Premi **Stats** in alto a destra e leggi **Tris**.

Il laboratorio ha una scena sua, `Laboratorio`: la camera dell'ingresso è sul **Display 6**, quella delle nicchie sul **Display 7**. Lì conta anche **Verts**, il numero dei vertici.

**Tris** conta i triangoli che Unity disegna in quel frame: solo quello che la camera vede, ma ogni passata. Quando l'editor è impostato per il PC, il progetto disegna la scena una volta in più per l'occlusione ambientale (SSAO), l'ombra morbida negli angoli della versione PC: per questo Stats mostra numeri più alti dei triangoli dei modelli. Con l'editor su Android, per esempio dopo una build per il visore, questa passata non c'è e i numeri sono più bassi, come sul visore. I budget sui cartelli sono pensati per l'editor impostato per il PC.

**RUFA ▸ Strumenti ▸ Conta triangoli (selezione)** dice invece quanti triangoli pesano gli oggetti selezionati, tutti i livelli di LOD compresi: è il peso degli asset, non quello che si disegna.

## 3. Le superfici piatte
Una superficie piatta ha bisogno di due triangoli, qualunque sia la sua grandezza. I triangoli in più non si vedono, ma costano. Succede quando si esporta con le suddivisioni ancora attive.
- Nel museo: seleziona `Pavimento` e, nel componente **Dense Grid**, scrivi `1` in **Cells**.
- Nel tuo programma 3D: togli le suddivisioni prima di esportare, o usa il modificatore **Decimate** in modalità **Planar**, che unisce le facce piatte.

Per vedere i triangoli: nella barra della Scene view, il menu della modalità di disegno, **Wireframe**.

## 4. Il dettaglio nella texture
La **normal map** è una texture che dice alla luce come è inclinata la superficie punto per punto: la luce vede rilievi che non ci sono. Meta consiglia di mettere il dettaglio nelle texture e non nei triangoli, e di chiedersi come appare un modello quando il dettaglio è cotto nella normal map: se la sagoma si riconosce ancora, i triangoli in più non servono.

Prova: nel materiale `Parquet`, sotto **Surface Inputs**, il numero accanto a **Normal Map** a `0` e poi di nuovo a `1`, guardando il pavimento di taglio.

## 5. La scultura
Un modello uscito da un programma di scultura, come ZBrush, ha milioni di triangoli. Una superficie curva ha bisogno di triangoli, ma quanti lo decide l'occhio: si riduce finché, dal punto in cui la guarda il visitatore, non si vede la differenza, e poi si torna di un passo.
- Nel museo: `Sfera`, componente **Dense Mesh**, campo **Triangles**.
- Nel tuo programma 3D: **Decimate**, oppure la **retopologia**, cioè rifare la rete del modello con meno poligoni e cuocere il dettaglio nella normal map.

## 6. Il LOD
Il **LOD**, livello di dettaglio, usa versioni sempre più semplici dello stesso modello man mano che occupa meno schermo.

Il componente **LOD Group** ha delle fasce: **LOD 0** è il modello pieno, **LOD 1** e **LOD 2** le versioni semplificate, **Culled** vuol dire che l'oggetto non si disegna più. Ogni soglia è una percentuale dell'altezza dello schermo: al 50% l'oggetto riempie metà dello schermo in altezza. Il salto visibile fra un livello e l'altro si chiama **popping**; si corregge spostando le soglie. Nel **Fade Mode** del LOD Group c'è anche la dissolvenza fra i livelli, che in URP va accesa anche nell'asset della pipeline.

Come si fa, su un prefab:
1. Le varianti: in Blender, Decimate al 50% e al 15%, esportate come `nome_LOD1.fbx` e `nome_LOD2.fbx` **nella stessa cartella** del modello. Nel museo ci sono già.
2. Doppio clic sul prefab nella finestra **Project**: il prefab si apre.
3. Seleziona l'oggetto principale nella Hierarchy e scegli **RUFA ▸ Strumenti ▸ LOD Group dalle varianti selezionate**. Le soglie di partenza sono 50%, 15% e 3%.
4. Nell'Inspector trascina la piccola camera sopra le fasce per vedere i cambi, e trascina i confini fra le fasce per spostare le soglie.
5. Esci dal prefab con la freccia in alto nella Hierarchy: si salva da solo, e la modifica vale per tutte le copie.

Gli oggetti piccoli scendono presto sotto la soglia di **Culled**: un orologio da tasca è sotto il 3% dello schermo già a un paio di metri. Lì la soglia va abbassata, finché non sparisce più niente di visibile.

In Unity 6.3 ci sono due modi di fare il LOD. Cambia chi prepara le versioni e quanto le controlli.
- **LOD Group**: un componente. Le versioni sono modelli separati, preparati da chi modella. Le soglie le sposti tu, puoi far sparire l'oggetto con **Culled**, e vedi quale livello sta usando.
- **Mesh LOD**: un'opzione dell'importazione, nell'Inspector del modello, sezione **Mesh LODs**, casella **Generate Mesh LODs**. Unity semplifica il modello da solo e mette i livelli dentro la mesh stessa, senza oggetti in più: è comodo e occupa meno memoria. Però non decidi tu come viene semplificato, Unity non mostra quale livello sta usando, e con lo static batching e con la GPU instancing usa sempre il modello pieno. Insieme a un LOD Group è sconsigliato.

Nella sala usiamo il LOD Group; nel laboratorio li provi tutti e due sui vasi.

## 7. Il culling
**Culling** vuol dire scartare quello che non serve disegnare.
- **Frustum culling**: quello che sta fuori dall'inquadratura Unity lo scarta da solo, a ogni frame.
- **Occlusion culling**: quello che sta dietro un muro Unity lo disegna, finché non calcoli prima chi nasconde chi. Il calcolo si chiama **bake**.

Il bake usa gli oggetti statici. **Occluder Static**: l'oggetto nasconde quello che ha dietro; deve essere opaco e non muoversi. **Occludee Static**: l'oggetto può essere nascosto. Un oggetto che si muove può essere nascosto, ma non nascondere. Meta lo consiglia negli ambienti chiusi, con muri grandi; in una scena piccola o aperta non serve.

Come si fa:
1. Seleziona la parte della scena, per esempio `Deposito`, e scegli **RUFA ▸ Strumenti ▸ Statici: imposta flag (selezione)**. I vetri restano senza Occluder Static.
2. **RUFA ▸ Strumenti ▸ Occlusion culling: bake**. A mano: **Window ▸ Rendering ▸ Occlusion Culling**, scheda **Bake**, pulsante **Bake**.
3. Per vedere il risultato: **Window ▸ Rendering ▸ Occlusion Culling**, scheda **Visualization**, e seleziona una camera nella Hierarchy. Nella Scene view resta solo quello che quella camera vede.

Se sposti un muro dopo il bake, rifai il bake.

## 8. Il laboratorio: le opere si preparano prima
Gran parte dell'ottimizzazione si fa nel programma 3D, prima di Unity. Nel laboratorio di restauro ci sono quattro postazioni.

**Esportare da Blender.** Ogni volta: **File ▸ Export ▸ FBX (.fbx)**; a destra, sotto **Include**, spunta **Selected Objects**, se no esporti tutta la scena; sotto **Transform**, in **Apply Scalings**, scegli **FBX Units Scale**, se no il modello arriva in Unity cento volte più piccolo. **Apply Modifiers** è già acceso: i modificatori, come il Decimate, li applica l'esportazione. Per importare: **File ▸ Import ▸ FBX (.fbx)**. I modelli del museo stanno in `sandbox/Assets/Demo/Models/Museo`, nella cartella della tua copia.

**Le nicchie: le facce che nessuno vede.** La GPU scarta le facce girate dall'altra parte (il **backface culling**), ma solo dopo aver calcolato i loro vertici: una faccia che nessuno vede costa comunque, e Stats la conta nei **Tris**. Un busto in una nicchia si vede solo da davanti, e tutto il retro, dalla nuca alla base, si può cancellare.
1. In Blender importa `busto_nicchia.fbx`, clicca il busto e premi **Tab** (Edit Mode), poi **3** (facce) e **Alt+Z** (vedi attraverso).
2. Guardalo di lato: nel cerchio degli assi in alto a destra, il pallino rosso **X**.
3. Trascina un rettangolo su tutta la parte dietro, dalla cima della testa alla base, un dito dietro le orecchie; **X ▸ Faces**; **Tab**.
4. Esporta sopra `busto_nicchia.fbx`, con lo stesso nome. I tre busti si aggiornano da soli.
Se hai tagliato troppo, ricomincia in un file nuovo (**File ▸ New ▸ General**, cancella il cubo), importa `marble_bust_01.fbx`, che è lo stesso busto, ed esportalo sopra `busto_nicchia.fbx`. Importato nella stessa scena di prima, Blender lo rinominerebbe e i busti perderebbero la mesh.

**Il tuo LOD: le versioni le fai tu.** Il modificatore **Decimate** toglie facce: **Collapse** unisce i vertici tenendo la forma e va bene quasi sempre; **Un-Subdivide** toglie le suddivisioni; **Planar** semplifica le parti piatte. **Ratio** è la parte da tenere: 0.5 è metà. Scendi finché la sagoma, il contorno del modello, regge.
1. In Blender importa `brass_vase_01.fbx`. **Properties ▸ Modifiers** (la chiave inglese) **▸ Add Modifier ▸ Generate ▸ Decimate**.
2. Ratio 0.5 circa, esporta come `brass_vase_01_LOD1.fbx`; Ratio 0.15 circa, esporta come `brass_vase_01_LOD2.fbx`. Nella stessa cartella del vaso.
3. In Unity, prefab `Vaso d'ottone` della cartella `Prefab` del museo, **RUFA ▸ Strumenti ▸ LOD Group dalle varianti selezionate**, poi esci dal prefab.
4. Sul **Display 6**, se un vaso della fila di sinistra cambia aspetto in modo visibile, riesporta quella versione con un Ratio più alto.

**Il Mesh LOD: lo fa Unity.** Seleziona `Assets/Demo/Models/Museo/antique_ceramic_vase_01`, nell'Inspector scheda **Model**, sezione **Mesh LODs**, **Generate Mesh LODs**, **Apply**. Confronta sul **Display 6** la fila di destra con la tua: triangoli, aspetto, controllo. Quale terresti, e perché, va nel file delle misure.

**Il vaso sfaccettato: i vertici.** Un vertice è condiviso dai triangoli intorno solo se per tutti ha la stessa posizione, la stessa **normale** (la direzione verso cui guarda la superficie) e le stesse UV. Uno spigolo duro, o una cucitura delle UV, lo sdoppia. Il vaso è stato importato con ogni spigolo duro: il riflesso della luce è a quadretti e i vertici sono quasi il triplo.
1. Seleziona `Assets/Demo/Models/Museo/vaso_sfaccettato`. Nell'Inspector, scheda **Model**, sezione **Geometry**: **Normals** su **Calculate**, **Smoothness Source** su **From Angle**.
2. Alza lo **Smoothing Angle**, per esempio a 60, e premi **Apply**. Rileggi **Verts**.
In Blender lo stesso si fa con **Object ▸ Shade Smooth** prima di esportare; da Blender 4.1 c'è anche **Shade Auto Smooth**.

## 9. Trucchi del mestiere
Nel programma 3D:
- **Il retro e il fondo**: mobili contro il muro, oggetti appoggiati, busti nelle nicchie. Le facce che toccano il muro o il pavimento si cancellano.
- **Le parti incastrate**: le facce di un pezzo infilato in un altro non si vedono. Uniscili, o cancella quello che sta dentro.
- **Le facce doppie**: un modello importato due volte, o due copie sovrapposte, costa il doppio e sfarfalla. In Edit Mode, **Mesh ▸ Clean Up ▸ Merge by Distance**.
- **Prima la sagoma**: i triangoli servono sul contorno; il dettaglio dentro la sagoma va nella normal map.
- **Niente triangoli lunghi e sottili, né minuscoli**: la GPU colora i pixel a gruppi di quattro, e un triangolo di pochi pixel la fa lavorare a vuoto. Da lontano lo risolve il LOD, da vicino una rete ordinata.
- **Il nome e la scala**: **FBX Units Scale**, e le versioni semplificate col nome del modello più `_LOD1` e `_LOD2`.

In Unity:
- **Il collider semplice**: per teletrasporto e presa basta un **Box Collider** o un **Capsule Collider**. Un **Mesh Collider** sul modello pieno fa lavorare la fisica su tutti i suoi triangoli: se serve la forma, dagli la versione LOD 2.
- **Il far plane**: la camera non disegna oltre il **Far** dei **Clipping Planes**; tienilo lungo quanto serve.
- **Il passaggio dolce**: nel LOD Group, **Fade Mode: Cross Fade** con **Animate Cross-fading** sfuma il salto fra due livelli; per un attimo Unity li disegna tutti e due.
- **La cartolina**: per le cose lontanissime basta un quad con l'immagine dell'oggetto, un **billboard**, o lo skybox.
- **Il terreno**: il **Terrain** ha il **Pixel Error**; più è alto, meno triangoli da lontano.
- **Mesh LOD o static batching**: con lo static batching (lezione 7) il Mesh LOD usa sempre il livello pieno.
- **In VR gli oggetti si avvicinano**: quello che si prende in mano arriva a venti centimetri dagli occhi e merita dettaglio e normal map; un soffitto a tre metri molto meno.
- **Le facce da due parti**: un materiale con **Render Face: Both** disegna anche il retro; solo per foglie e stoffe sottili.

Le versioni del vaso e il taglio dei busti stanno in `Assets/Demo/Models/Museo`: al prossimo `git aggiorna` tornano come prima. Sono un esercizio; le tue foto e il file delle misure restano. I LOD per il tuo progetto falli sui tuoi modelli, nella cartella del progetto.

## 10. Il giro del curatore
Per ogni sala, sempre lo stesso giro:
1. il display della sala e **Tris**, scritto nel file delle misure;
2. la foto del prima, col pulsante del tutorial;
3. la cura;
4. di nuovo **Tris**, e la foto del dopo.

Le regole: non spegnere, non cancellare, non spostare niente. Poi un compagno fa il **curatore**, cioè chi si occupa della mostra. Gli mostri le due foto a tutto schermo, nell'ordine che decidi tu e senza fargli vedere i nomi dei file, e lui dice quale è quella ottimizzata e perché. Se ti indica una differenza che c'è davvero, torna a regolare. Se non trova differenze, o tira a indovinare, il lavoro è buono.

Il giardino e le curiosità stanno nella scena `Museo`; il deposito, per ultimo, nella scena `Deposito`, con una lista sua nel tutorial. Quando Unity chiede se salvare il museo, rispondi **Save**. Alla fine di ogni sala salva con **Ctrl+S**.

## 11. La consegna
Tutto in `Consegne/L06/`:
- `misure.md`, dal pulsante del tutorial, senza nessun «da scrivere»;
- le foto del museo: `salone-`, `giardino-`, `deposito-` e `curiosita-prima.jpg` e `-dopo.jpg`;
- le foto delle nicchie del laboratorio: `nicchie-prima.jpg` e `nicchie-dopo.jpg`;
- le foto del tuo progetto: `progetto-prima.jpg` e `progetto-dopo.jpg`, dalla tua `Camera foto`.

Per arredare il blockout, le opere del museo, coi loro materiali, sono in `Assets/Samples/RUFA Core/Lezione 06 - Geometria/Prefab`; i mobili in `Assets/Demo/Models`.

A fine lezione, `git salva "Lezione 06"`.

## 12. I budget nella vita vera
- **VRChat**, avatar per Quest e mobile: fino a 7.500 triangoli è Excellent, 10.000 Good, 15.000 Medium, 20.000 Poor; oltre, gli altri non vedono l'avatar.
- **Roblox**: un accessorio rigido al massimo 4.000 triangoli, una singola mesh al massimo 20.000.
- **Unity Asset Store**: nella descrizione di un modello va scritto il numero dei poligoni, e i modelli con i LOD devono avere prefab col LOD Group. **Fab**, il negozio di Epic, chiede la stessa cosa.

## 13. Problemi comuni
- **Tris non cambia.** Stai guardando il display sbagliato, o la Scene view è davanti alla Game view.
- **Dopo il bake non cambia niente.** Gli oggetti non sono statici: rifai **Statici: imposta flag** sulla parte giusta, poi di nuovo il bake.
- **Le cose spariscono nel posto sbagliato.** Hai spostato qualcosa dopo il bake: rifallo.
- **Il LOD Group non c'è sulle copie.** L'hai messo su una copia e non sul prefab, oppure non sei uscito dal prefab.
- **Lo strumento dei LOD dice che mancano le varianti.** Le varianti devono stare nella stessa cartella del modello, coi nomi `_LOD1` e `_LOD2`.
- **Gli scaffali si svuotano.** La soglia di **Culled** è troppo alta per oggetti così piccoli: abbassala nel prefab, un poco alla volta, finché gli scaffali in fondo non sono di nuovo pieni.
- **Il compito resta Da fare, e la cura è giusta.** Nella sala qualcosa è stato spostato o spento: annulla con **Ctrl+Z**, o chiama il docente.
- **I tuoi numeri sono la metà di quelli del compagno.** Il tuo editor è impostato su Android e il suo per il PC: la passata dell'SSAO c'è solo sul PC.
- **Il busto, o una versione del vaso, è minuscolo.** L'hai esportato senza **Apply Scalings: FBX Units Scale**: rifai l'esportazione.
- **Blender ha esportato anche il cubo e la luce.** Mancava **Selected Objects**: rifai l'esportazione col solo modello selezionato.
- **Nelle nicchie la pagina resta Da fare.** Hai tolto solo la nuca: il rettangolo deve prendere tutto il retro, dalla testa alla base.
- **Lo Smoothing Angle non cambia niente.** **Smoothness Source** deve essere **From Angle**, e alla fine va premuto **Apply**.
- **La foto è nera o vuota.** Premi di nuovo il pulsante: subito dopo aver aperto la scena, la prima foto può uscire senza oggetti.

## Fonti
- Meta, «Performance and optimization» per Unity, i budget per Quest 3 e 3S: https://developers.meta.com/vr/documentation/unity/unity-perf/
- Meta, «Art and performance», i budget sono totali per frame: https://developers.meta.com/vr/design/art-and-performance/
- Meta, «Art Direction for All-in-One VR Performance»: https://developers.meta.com/vr/documentation/unity/po-art-direction/
- Meta, «Unity best practices» e le regole per le scene su mobile: https://developers.meta.com/vr/documentation/unity/unity-best-practices-intro/ e https://developers.meta.com/vr/documentation/unity/unity-mobile-performance-intro/
- Unity 6.3, LOD Group: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/lod/lod-group/class-lodgroup
- Unity 6.3, Mesh LOD: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/lod/mesh-lod/mesh-lod-introduction
- Unity 6.3, generare i Mesh LOD all'importazione: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/lod/mesh-lod/mesh-lod-generate-lods
- Unity 6.3, la scheda Model dell'importazione (Normals, Smoothness Source, Smoothing Angle): https://docs.unity.com/en-us/engine/6000.3/manual/assets-and-media/asset-types/models/importing/fbximporter-model
- Blender 4.2, il modificatore Decimate: https://docs.blender.org/manual/en/4.2/modeling/modifiers/generate/decimate.html
- Blender 4.2, importare ed esportare FBX: https://docs.blender.org/manual/en/4.2/addons/import_export/scene_fbx.html
- Meta, i consigli di Arm sugli asset per la VR (triangoli piccoli e sottili): https://developers.meta.com/vr/blog/top-tips-from-arm-for-vr-asset-optimization/
- Unity 6.3, occlusion culling: https://docs.unity.com/en-us/engine/6000.3/manual/cameras/occlusion-culling/getting-started
- Unity 6.3, la finestra Statistics: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/profiling/rendering-statistics
- VRChat, i ranghi degli avatar: https://creators.vrchat.com/avatars/avatar-performance-ranking-system/
- Roblox, accessori e mesh: https://create.roblox.com/docs/avatar/rigid-accessories/specifications e https://create.roblox.com/docs/art/modeling/specifications
- Unity Asset Store, le regole di pubblicazione: https://assetstore.unity.com/publishing/submission-guidelines
- Fab, i requisiti tecnici: https://www.fab.com/technical-requirements
