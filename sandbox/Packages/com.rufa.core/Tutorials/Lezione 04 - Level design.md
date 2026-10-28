Lezione 4 · Level design e blockout. Le note prima del primo capitolo non si vedono nella finestra. Capitoli insieme 0-3 (concept compreso) entro 130' per lo studente lento, verifica entro 60': dopo ogni modifica, la riga "Insieme, studente lento" di tools/stima-tempi.py (il suo OK è sul limite generale di 150').

# 0 · La camminata critica

## Cosa farai in questa lezione
Oggi progetti lo spazio. Il **blockout** è il livello fatto di volumi semplici, senza grafica: serve a provare misure, percorsi e tempi prima dei modelli veri. Coi cubi in Unity, in Blender o in Maya: quello che impari oggi vale per tutti e tre.

Prima curi la **Casa storta**, una casetta di cubi con un difetto per postazione. Poi scrivi il concept del tuo progetto e lo presenti al docente; mentre aspetti il turno fai la verifica, e alla fine costruisci il blockout del tuo livello.

## Importa la palestra
Premi il pulsante: la palestra arriva in `Assets/Samples/RUFA Core/Lezione 04 - Level design` e si apre la Casa storta.
> azione: Importa la palestra | importa-sample | Lezione 04 - Level design
> verifica: scena-aperta Casa storta
> lavoro: 1 1

## Il simulatore
Le prove in Play si fanno col simulatore, come nelle lezioni 2 e 3: **RUFA ▸ Play mode ▸ Simulatore VR**.
> verifica: simulatore-acceso

## Entra in Play
Premi Play. Le stanze della Casa storta sono in fila, e ogni difetto ha il suo cartello «Postazione N». Cercali come li troverebbe chi gioca: cammina, guardati intorno e annota cosa stona nello spazio, non nella grafica.

I tasti sono quelli della lezione 2: **Shift + I J K L** è la levetta sinistra, per camminare; **I J K L** la destra, per girare a scatti e per il teleport: tieni **I**, punta l'arco col mouse (tasto destro tenuto) e lascia **I**. Col tasto destro tenuto giri anche lo sguardo.
> verifica: in-play
> attesa: 15 entrata in Play
> lavoro: 1 1

## Sintomo 1: la partenza
Appena entri in Play guardi un muro: la porta della casa, la prima cosa da vedere, ce l'hai alle spalle.
> lavoro: 1 1

## Sintomo 2: la porta d'ingresso
Girati. Accanto alla porta ci sono il manichino del corso, alto 1,75 m, e una porta vera da 80 × 210 cm: quella della casa è larga il doppio e alta come un portone. Nel visore sembra un buco nel muro, e la scala di tutta la stanza diventa incerta.
> lavoro: 1 1

## Sintomo 3: il soffitto
Entra nel soggiorno: il soffitto è a 2,2 m. Sopra la testa resta mezzo metro e la stanza schiaccia; a chi è alto, ancora di più.
> lavoro: 1 1

## Sintomo 4: la lanterna
A sinistra c'è una lanterna da prendere, a terra: nel visore, per prenderla bisogna inginocchiarsi.
> lavoro: 1 1

## Sintomo 5: la porta bassa
In fondo al soggiorno c'è una porta con l'architrave a 1,5 m. Camminaci contro con **Shift + I**: ti fermi. Ora teletrasportati oltre: passi. Il corpo del rig è alto quanto la tua testa, e il teleport non lo guarda.
> lavoro: 2 1

## Sintomo 10: sbirciare
Oltre la porta bassa, nel corridoio, mettiti davanti al cartello «Postazione 10», col muro davanti. Tieni premuto **W** per un secondo, non di più: col simulatore W A S D muovono la testa come se ti sporgessi col corpo, e la testa entra nel muro. Dall'altra parte c'è un ripostiglio chiuso, con un codice sul muro: in un'escape room vorrebbe dire leggere la soluzione. Con **S** torni indietro. Se W non muove la testa, premi prima **Tab**.
> lavoro: 2 1

## Sintomo 6: le scale
In fondo al corridoio tre gradini da 20 cm salgono alla terrazza. Salili camminando: la visuale sale a scatti, e nel visore ogni scatto è un piccolo salto della testa.
> lavoro: 1 1

## Sintomo 7: l'arco
Dalla terrazza guarda in alto: sopra la casa passa un arco enorme. È un modello di Blender alto 2,4 m, arrivato in Unity 100 volte più grande, e senza collider: anche a misura giusta, ci passeresti attraverso.
> lavoro: 1 1

## Sintomo 9: la terrazza
Sulla terrazza prova a teletrasportarti sul suo pavimento: l'arco del teleport resta rosso. Ci arrivi solo camminando.
> lavoro: 1 1

## Sintomo 8: la stanza buia
Entra nella stanza in fondo alla terrazza: il corridoio a gomito è nero, e la stanza è buia e tutta uguale. Niente dice dove andare, e l'uscita, in fondo a destra, non si vede.
> lavoro: 1 1

## Sintomo 11: il panorama
Esci dalla stanza buia: oltre l'uscita c'è un balcone su una valle, con case, una strada e una torre lontana. Ma la vista è piatta: il cielo è quello di serie, e le montagne lontane hanno lo stesso contrasto delle case vicine.
> lavoro: 1 1

## Esci dal Play
Esci dal Play. Gli undici difetti sono di spazio, non di grafica: misure, il corpo del rig, un modello importato, il teleport, la luce, la testa nei muri, il panorama. Li curi uno alla volta.
> verifica: fuori-play

# 1 · Misure e corpo

## Un'unità è un metro
In Unity **1 unità = 1 metro**, e nel visore lo senti: fisica, luce e corpo ragionano in metri. Un cubo con **Scale** 1 misura un metro; un muro lungo 4 m, alto 3 e spesso 20 cm ha Scale 4, 3, 0,2.

L'altezza della testa invece non la decidi tu: il rig nasce coi piedi dove sta il `Rig Bootstrap`, e la testa la mette il visore, all'altezza vera di chi gioca. Per questo le stanze si provano nel visore: misure giuste per te possono stare strette a chi è più alto.

## Le misure di casa
Per le stanze valgono le misure del mondo reale: Meta non ne dà altre, e chiede di provarle nel visore.

- **Porta**: 80–90 × 210 cm;
- **Soffitto**: 2,7–3,0 m;
- **Corridoio**: almeno 1,2 m;
- **Gradino**: alzata 16–18 cm, pedata 28–30 cm;
- **Oggetto da prendere**: tra 0,7 e 1,3 m da terra.

Il manichino e la porta da 80 × 210 cm sono i riferimenti: tienili vicino a quello che costruisci.

## Lo snap
Per costruire a misura i pezzi si spostano a scatti precisi:
- tenendo **Ctrl** mentre trascini, l'oggetto si sposta a passi fissi: regola il passo a 0,1 (10 cm) nel menu della griglia, in alto nella Scene view;
- tenendo **V** mentre trascini, prendi un vertice e lo appoggi su quello di un altro oggetto: due muri si toccano senza fessure.
Quando conosci i valori, scriverli nell'Inspector è ancora più preciso.

## La porta d'ingresso
Sintomo 2. La porta d'ingresso è un vano di 1,6 × 2,6 m fra il muro a sinistra, la `Spalla est` e l'`Architrave d'ingresso`. Portalo a 0,9 × 2,1 m:
- `Spalla est`: **Position X** 1,725 e **Scale X** 2,55, così il suo bordo arriva a 45 cm dal centro;
- `Architrave d'ingresso`: **Position** (0, 2,65, 5) e **Scale** (0,9, 1,1, 0,2), così la sua faccia di sotto va a 2,1 m.
Salva.
> evidenzia: Inspector | Position | Spalla est
> verifica: varco Misura porta d'ingresso | 0,8 | 1,0 | 2,0 | 2,2
> lavoro: 3 2

## Il soffitto
Sintomo 3. Il `Soffitto del soggiorno` è un cubo spesso 10 cm con la faccia di sotto a 2,2 m. Mettilo a **Position Y** 2,85: la faccia di sotto va a 2,8 m. Salva.
> evidenzia: Inspector | Position | Soffitto del soggiorno
> verifica: altezza-libera Misura soggiorno | 2,6 | 3,1
> lavoro: 2 1

## La lanterna
Sintomo 4. Meta chiede di mettere gli oggetti da usare a portata di mano. Porta la `Lanterna` sul `Tavolo`, alto 75 cm: **Position** (1,5, 0,85, 9). Salva.
> evidenzia: Inspector | Position | Lanterna
> verifica: altezza Lanterna | 0,7 | 1,3
> lavoro: 2 1

## Il corpo del rig
Nella cartella della palestra apri il prefab `XR Origin (lezione 4)`, il rig che nasce dal `Rig Bootstrap`. Sulla radice c'è un **Character Controller**: il corpo, una capsula che non attraversa i collider. Prima di ogni passo XR Interaction Toolkit gli dà l'altezza della testa; il teleport invece porta il rig a destinazione senza guardare la capsula. Due campi decidono dove si sale camminando:
- **Step Offset**, il gradino più alto: qui 30 cm, quindi un gradino da 20 sì e la terrazza da 60 no. Il rig di serie degli Starter Assets ha 50 cm; il manuale di Unity consiglia da 10 a 40;
- **Slope Limit**, la pendenza più ripida: qui 45°.
Meta consiglia rampe dolci invece delle scale e il teleport per i dislivelli: col movimento continuo ogni gradino è uno scatto della testa, e la vignetta della lezione 2 lo attenua. Chiudi il prefab senza cambiare niente.

## L'area di gioco
Nel visore chi gioca si muove anche col corpo vero, dentro il confine disegnato nella sua stanza. Meta chiede di progettare per 2 × 2 m in piedi e per 1 × 1 m da fermi, e circa metà dei giocatori sta seduta. Per lo Store, da seduti nessuna azione deve chiedere di alzarsi, di chinarsi fino a terra o di girarsi oltre 90°. Nel tuo livello:
- ogni enigma sta dentro 2 × 2 m, con quello che serve a portata di mano;
- niente da raccogliere da terra: quello che può cadere si deve poter prendere da lontano col raggio;
- gli enigmi davanti a chi gioca, non alle spalle.
Il pulsante **Aggiungi i riferimenti di scala** disegna anche il quadrato di 2 × 2 m a terra, intorno alla partenza.

## La porta bassa
Sintomo 5. L'`Architrave basso` ha la faccia di sotto a 1,5 m: portala a 2,1 m con **Position Y** 2,65 e **Scale Y** 1,1. Salva, e in Play rifai la prova: camminando ora passi.
> evidenzia: Inspector | Position | Architrave basso
> verifica: varco Misura porta bassa | 0,8 | 1,0 | 2,0 | 2,2
> lavoro: 3 1

## Le scale
Sintomo 6. Il trucco del mestiere è una **rampa invisibile** appoggiata sugli spigoli dei gradini: i gradini si vedono, i piedi salgono sulla rampa. Crea un cubo (**GameObject ▸ 3D Object ▸ Cube**) e chiamalo `Rampa`:
- **Position** (0, 0,3, 16), **Rotation** (−26,57, 0, 0), **Scale** (1,4, 0,02, 1,35): parte dal pavimento 40 cm prima del primo gradino e arriva allo spigolo dell'ultimo;
- togli la spunta al suo **Mesh Renderer**: non si vede, ma il **Box Collider** resta.
Salva, e in Play risali camminando: la visuale sale liscia.
> verifica: percorso-liscio Misura scale inizio | Misura scale fine | 0,03
> lavoro: 4 2

## La testa nel muro
Sintomo 10. La capsula del rig ferma i passi fatti con la levetta, non la testa: nessun gioco può impedire a chi gioca di sporgersi. Meta chiede di non spostare né bloccare la camera, e di spegnere la vista quando la testa entra in un muro: chi gioca vede nero e impara da solo a tirarsi indietro. XR Interaction Toolkit non lo fa: nel corso c'è il componente **Head Blackout**. Spegne la vista anche se la testa passa del tutto oltre un muro sottile, finché non torna dalla sua parte.
Nella cartella della palestra apri il prefab `XR Origin (lezione 4)` e alla radice **Add Component ▸ Head Blackout**. **Radius** è la distanza dal muro, in metri, a cui la vista si spegne: lascia 0,1. Salva il prefab, e in Play rifai la prova del sintomo 10, tenendo W per un secondo, non di più: nel muro vedi nero, e con **S** torni indietro e la vista torna.
Per l'escape room vale anche al contrario: muri spessi, e niente indizi subito dietro una parete sottile.
> verifica: prefab-componente XR Origin (lezione 4) |  | HeadBlackout
> lavoro: 3 1

# 2 · Modelli, teleport e sguardo

## Gli Import Settings
Se il blockout lo fai in Blender o in Maya, conta come Unity legge il file. Seleziona un modello nella finestra Project: nell'Inspector, scheda **Model**:
- **Scale Factor** moltiplica tutto il modello;
- **Convert Units** porta in metri l'unità del file: Maya lavora in centimetri, e con Convert Units acceso 210 cm diventano 2,1 m;
- **Bake Axis Conversion**: in Blender l'alto è l'asse Z, in Unity è la Y, e un modello di Blender arriva in scena con la Rotation X a −90 per stare dritto. Acceso, Unity raddrizza il modello una volta per tutte, e in scena la sua Rotation è 0;
- **Generate Colliders** dà al modello i suoi Mesh Collider: senza, corpo e teleport ci passano attraverso.
Dopo ogni cambio premi **Apply**. La prova più semplice: esporta un cubo da 1 m e mettilo accanto a un cubo di Unity. Esporta sempre in FBX: i file .ma e .mb di Maya Unity li apre con regole sue.

## L'arco
Sintomo 7. Nella cartella della palestra seleziona `Arco`: è fatto in centimetri, ma ha **Convert Units** spento, e Unity legge 240 cm come 240 m. Accendi **Convert Units** e **Generate Colliders**, poi **Apply**. L'arco torna alto 2,4 m, sul balcone oltre l'uscita della stanza buia, dove incornicia la vista. E ora ha i collider: se cammini contro una gamba ti fermi, come contro un muro, mentre prima ci passavi attraverso.
> verifica: dimensione Arco | y | 2,3 | 2,5
> lavoro: 3 1

## La terrazza
Sintomo 9. Il teleport arriva solo su una **Teleportation Area** del layer **Teleport**, come nella lezione 2. Seleziona la `Terrazza`, **Add Component ▸ Teleportation Area**, e in **Interaction Layer Mask** lascia solo **Teleport**. Salva. Ripasso della lezione 2: su un'area con le pareti, **Filter Selection By Hit Normal** evita che puntare la parete porti in cima.
> evidenzia: Inspector | Interaction Layer Mask | Terrazza
> verifica: teleport-sotto Misura terrazza
> lavoro: 2 1

## La partenza
Sintomo 1. Il rig nasce dove sta il `Rig Bootstrap`, rivolto come lui. Selezionalo e porta **Rotation Y** a 0, verso la porta d'ingresso. Salva: in Play la prima cosa che vedi è la porta. Nel visore conta anche come sei girato nella tua stanza: tenendo premuto il tasto Meta del controller destro la vista si riallinea.
> evidenzia: Inspector | Rotation | Rig Bootstrap
> verifica: rivolto-verso Rig Bootstrap | Misura porta d'ingresso | 30
> lavoro: 1 1

## Gli anchor
Meta chiama hotspot i punti d'arrivo del teleport: li mette davanti alle porte e agli enigmi, e l'arco ci si aggancia come a una calamita. In XR Interaction Toolkit sono i **Teleportation Anchor** della lezione 2: con **Match Orientation** su Target Up And Forward arrivi rivolto come l'anchor. Nel tuo livello:
- un anchor davanti a ogni enigma, rivolto all'enigma;
- almeno a mezzo metro dai muri: arrivare col naso sulla parete è il modo più facile di sbirciare;
- un anchor in cima e in fondo a ogni scala, come chiede Meta per i dislivelli.

## Guidare lo sguardo
In VR chi gioca guarda dove vuole: il livello lo guida con lo spazio, non con le frecce.
- **Luce e contrasto**: l'occhio va dove c'è luce. Meta fa l'esempio della porta che si illumina perché tu la apra; i cambi di luce bruschi invece infastidiscono.
- **Il dettaglio**: lo sguardo va dove c'è più dettaglio, e un fondo troppo ricco distrae.
- **Il campo visivo**: le cose importanti stanno davanti, non di lato o alle spalle.
- **Un landmark per stanza**: un oggetto grande e riconoscibile dice dove sei senza mappa.
- **Linee e cornici**: porte e corridoi puntano verso la meta.
- **I suoni**: l'audio spaziale richiama da dove viene; lo vedremo nella lezione 12.

## Il ritmo
- **Compressione e apertura**: un passaggio stretto e poi una stanza grande danno ritmo; stanze tutte uguali annoiano.
- **Le distanze**: in VR dieci metri di corridoio vuoto sono lunghi. Nel walking simulator metti un motivo di interesse ogni 5–8 m.
- **I muri**: Meta consiglia spazi aperti, e di non far camminare chi gioca rasente ai muri.
- **I testi nel mondo**: almeno a mezzo metro, comodi intorno a un metro.

## La luce alla svolta
Sintomo 8, prima parte. Nei corridoi bui gli horror come Resident Evil 2 e Left 4 Dead non illuminano il tratto dritto, ma la svolta: la parete che hai davanti quando giri si accende e dice "di qua", mentre il resto resta in penombra. Una luce sola tira, troppe confondono. Clicca nel vuoto della Hierarchy, così non è selezionato niente, e aggiungi **GameObject ▸ Light ▸ Point Light**: chiamala `Luce della svolta`, **Position** (−2,6, 2,6, 24), sull'angolo dove il corridoio entra nella stanza, **Range** 4. Salva, e in Play percorri il corridoio: in fondo il muro si accende e la svolta si vede.
Sul Quest 3S, che ha uno schermo LCD, quello che è più scuro di 13 su 255 diventa nero pieno: nel tuo livello fai le zone buie un po' più chiare di come le vedi sul monitor.
> verifica: luce-vicino Svolta | 1,5
> lavoro: 3 1

## La luce sull'uscita
Sintomo 8, seconda parte. Ora una luce che porti all'uscita: ancora con niente selezionato nella Hierarchy, **GameObject ▸ Light ▸ Point Light**, chiamala `Luce dell'uscita`, **Position** (2,55, 2,6, 28,3), **Range** 4. È la porta che si illumina dell'esempio di Meta. Salva, e in Play rifai il giro.
> verifica: luce-vicino Uscita | 1,5
> lavoro: 3 1

## Il panorama
Sintomo 11. Dopo il buio, l'apertura: è la compressione e apertura del ritmo, nella sua forma più forte. Una vista si costruisce in tre piani: vicino la cornice (l'arco, il parapetto), in mezzo le case e la strada, che danno la scala, lontano la torre e le montagne. Nel visore la distanza si legge solo coi riferimenti in mezzo: una montagna sola sull'orizzonte sembra un fondale. La profondità la dà la nebbia, che schiarisce quello che è lontano.
Apri **Window ▸ Rendering ▸ Lighting**, scheda **Environment**:
- **Skybox Material**: `Cielo`, un cielo di Poly Haven (CC0) senza terreno, perché il terreno lo mette la scena;
- **Fog** acceso, **Mode** Linear, **Start** 50, **End** 1000, e il **Color** preso col contagocce sull'orizzonte del cielo.
Salva, e in Play esci dalla stanza buia.
> verifica: panorama Cielo | 500
> lavoro: 4 2

# 3 · Il concept

## Il concept
Il concept è la mezza pagina che presenti al docente: è la prima milestone del corso, e senza approvazione non si va avanti. Premi il pulsante: crea `Consegne/L04/concept.md` dal modello, nella cartella del tuo repository, e lo apre (se Windows chiede con quale programma, scegli Blocco note). Sostituisci ogni «da scrivere»:
- **titolo e genere**: walking simulator o escape room;
- **ambientazione e tono**, in due righe: cosa prova chi gioca;
- **struttura**: per il walking simulator i 3–5 momenti in ordine, ognuno con cosa si vede, cosa si sente e il trigger che lo avvia; per l'escape room la catena degli enigmi, 3–5 passaggi "trovo X → uso X su Y → si apre Z", coi mattoncini della lezione 3;
- **pianta**: in scala, col percorso e i punti di interesse numerati, salvata come `pianta.png` nella stessa cartella;
- **perimetro**: stanze e misure, asset (solo CC0: Poly Haven, ambientCG, Kenney), cosa tagli se manca il tempo.
> azione: Crea il concept | crea-concept | Consegne/L04/concept.md
> verifica: file-compilato Consegne/L04/concept.md | «da scrivere»
> lavoro: 10 7

## Il pitch
Quando il docente ti chiama, porta il concept aperto: hai 3 minuti. Il docente guarda quattro cose:
- è fattibile in 11 settimane, da solo: 2–3 stanze per l'escape room, 3–5 momenti per il walking simulator;
- si costruisce coi mattoncini della lezione 3, senza programmazione nuova;
- gli asset sono CC0, e sai da dove vengono;
- il percorso è chiaro: dove si parte, dove si va, quando è finita.
Esce approvato, approvato con modifiche (le scrivi nel concept) o da rifare entro la lezione 5. Mentre aspetti il turno fai la verifica.

# Da soli · Verifica

## Il rifugio
Ora tocca a te, senza istruzioni passo passo. Premi il pulsante: si apre la scena `Verifica`. A terra c'è la pianta di un rifugio: ingresso, corridoio, sala con la pedana in fondo. Ci sono i pavimenti e i muri esterni; il resto lo costruisci tu, coi cubi o importando un modello.

Le regole:
- **non spostare niente di `Ambiente`**: pianta, muri, pavimenti, pedana, panca;
- quello che costruisci mettilo dentro `Il mio blockout`, già in scena: sta nell'origine, e le Position che scrivi sono quelle del mondo;
- puoi **aggiungere** componenti agli oggetti di `Ambiente`, come la Teleportation Area ai pavimenti;
- ogni misura si legge sui collider: un modello importato senza collider non conta.

Qui sotto c'è cosa devi ottenere, non come; se ti blocchi, sotto ogni compito c'è **Mostra la soluzione**: il compito conta lo stesso, ma resta segnato. I compiti si spuntano da soli mentre lavori. Le prove in Play col **Simulatore VR**, o col visore e Quest Link.
> azione: Apri la verifica | apri-scena | Lezione 04 - Level design | Verifica

## La porta d'ingresso
Fra l'ingresso e il corridoio, sulla linea della pianta, manca il muro con la porta: la porta va dove la pianta lascia il varco, 90 × 210 cm.

**Risultato atteso:** nel muro una porta larga 0,8–1,0 m e alta 2,0–2,2 m.
> verifica: varco Misura porta | 0,8 | 1,0 | 2,0 | 2,2
> soluzione: I pezzi vanno dentro `Il mio blockout`: clic destro su di lui nella Hierarchy, **3D Object ▸ Cube**.
> soluzione: Un cubo `Muro d'ingresso - ovest`: **Position** (−1,225, 1,6, 4), **Scale** (1,55, 3,2, 0,2).
> soluzione: Un cubo `Muro d'ingresso - est`: **Position** (1,225, 1,6, 4), **Scale** (1,55, 3,2, 0,2).
> soluzione: Un cubo `Architrave`: **Position** (0, 2,65, 4), **Scale** (0,9, 1,1, 0,2). Salva.
> lavoro: 6 3

## Il corridoio
Il corridoio ha il muro ovest; l'est è solo una linea sulla pianta.

**Risultato atteso:** il corridoio è largo da 1,2 a 2 m.
> verifica: larghezza Misura corridoio | 1,2 | 2,0
> soluzione: Un cubo `Corridoio - muro est`: **Position** (0,8, 1,6, 7), **Scale** (0,2, 3,2, 6). Salva.
> lavoro: 3 1

## Il soffitto della sala
La sala non ha soffitto.

**Risultato atteso:** il soffitto della sala è fra 2,6 e 3,1 m.
> verifica: altezza-libera Misura sala | 2,6 | 3,1
> soluzione: Un cubo `Soffitto della sala`: **Position** (0, 2,85, 13), **Scale** (6, 0,1, 6): la faccia di sotto è a 2,8 m. Salva.
> lavoro: 3 1

## La rampa
In fondo alla sala c'è la pedana, alta 60 cm. La pianta segna l'impronta della rampa, 1,2 × 2,4 m.

**Risultato atteso:** dal pavimento della sala alla pedana si sale camminando, senza scatti.
> verifica: percorso-liscio Misura rampa inizio | Misura rampa fine | 0,03
> soluzione: Un cubo `Rampa`: **Position** (0, 0,3, 12,8), **Rotation** (−14,04, 0, 0), **Scale** (1,2, 0,02, 2,48): va dal pavimento al bordo della pedana. Salva.
> lavoro: 6 3

## Il teleport
I pavimenti non accettano il teleport.

**Risultato atteso:** ingresso, corridoio, sala e pedana accettano il teleport.
> verifica: teleport-sotto Misura ingresso | Misura corridoio | Misura sala | Misura pedana
> soluzione: In `Ambiente` seleziona insieme `Pavimento ingresso`, `Pavimento corridoio`, `Pavimento sala` e `Pedana` (Ctrl + clic).
> soluzione: **Add Component ▸ Teleportation Area**, e in **Interaction Layer Mask** solo **Teleport**. Salva.
> lavoro: 3 1

## La partenza
Il `Rig Bootstrap` guarda il muro alle spalle.

**Risultato atteso:** il rig nasce rivolto alla porta d'ingresso.
> verifica: rivolto-verso Rig Bootstrap | Misura porta | 30
> soluzione: Seleziona `Rig Bootstrap` e porta **Rotation Y** a 0. Salva.
> lavoro: 2 1

## La luce sull'uscita
L'uscita è una porta nel muro di fondo, sopra la pedana.

**Risultato atteso:** una luce accesa a non più di 1,5 m dall'uscita.
> verifica: luce-vicino Uscita | 1,5
> soluzione: **GameObject ▸ Light ▸ Point Light**, chiamala `Luce dell'uscita` e mettila in `Il mio blockout`.
> soluzione: **Position** (2, 2,6, 15,3), **Range** 4. Salva.
> lavoro: 3 1

## L'anchor sulla pedana
Chi arriva sulla pedana col teleport deve trovarsi davanti all'uscita.

**Risultato atteso:** sulla pedana, a meno di 3 m dall'`Uscita`, un Teleportation Anchor sul layer Teleport, con Match Orientation Target Up And Forward, rivolto all'uscita.
> verifica: anchor-verso Uscita
> soluzione: Clic destro su `Il mio blockout`, **XR ▸ Teleportation Anchor**, e chiamalo `Anchor della pedana`.
> soluzione: **Position** (2, 0,61, 14,9), sulla pedana davanti all'uscita; **Rotation** (0, 0, 0), così guarda l'uscita; **Scale** (0,1, 1, 0,1): il menu crea un piano di 10 × 10 m, e così diventa di 1 × 1 m.
> soluzione: Nel componente Teleportation Anchor: **Match Orientation** Target Up And Forward, **Interaction Layer Mask** solo Teleport. Salva.
> lavoro: 3 1

## Sulla pedana
**Risultato atteso:** in Play arrivi sulla pedana, camminando su per la rampa o col teleport.
> verifica: sopra-oggetto Pedana
> soluzione: Premi Play. Camminando: **Shift + I** lungo il corridoio e su per la rampa.
> soluzione: Col teleport si va a salti: tieni **I**, punta col mouse (tasto destro tenuto) il corridoio, poi la sala, poi la pedana, e lascia **I** a ogni salto.
> lavoro: 2 1

## L'ambiente intatto
**Risultato atteso:** nessun oggetto di `Ambiente` spostato, ruotato o scalato.
> verifica: ambiente-intatto Ambiente
> soluzione: Se il compito resta ✗, un oggetto di `Ambiente` è stato spostato, ruotato o scalato: **Ctrl+Z** finché torna ✓.

## Extra: la panca da Blender
In `Ambiente` c'è una `Panca`, a terra vicino al muro ovest della sala: si vede appena. Chi l'ha modellata lavorava in una scena in centimetri pensando in metri: misura 0,45 unità, cioè 4,5 mm. È un punto in più, fuori dal punteggio.

**Risultato atteso:** la panca è alta 45 cm, coi suoi collider.
> verifica: dimensione Panca | y | 0,4 | 0,5
> soluzione: Nella cartella della palestra seleziona `Panca`, scheda **Model**.
> soluzione: **Scale Factor** 100, **Generate Colliders** acceso, **Apply**.

# Da soli · Il tuo progetto

## La tua scena
Premi il pulsante: si apre la tua scena `Progetto`, o Unity la crea se non c'è ancora. Il blockout del tuo livello lo costruisci qui, dopo il pitch.
> azione: Crea la scena del progetto | crea-progetto

## I riferimenti di scala
Premi il pulsante: davanti al `Rig Bootstrap` della scena aperta arrivano il manichino da 1,75 m, la porta da 80 × 210 cm e, a terra, il quadrato di 2 × 2 m dell'area di gioco. Tienili in scena mentre costruisci.
> azione: Aggiungi i riferimenti di scala | riferimenti-scala

## Il blockout
Costruisci il livello del concept, solo con volumi: coi cubi, oppure in Blender o in Maya.
- Muri spessi 20 cm, alle misure di casa; ogni oggetto col nome della sua stanza, per esempio `Ingresso - muro nord`.
- I modelli importati con Convert Units e Generate Colliders, e la prova del cubo da 1 m.
- Una Teleportation Area su ogni pavimento, e il `Rig Bootstrap` dove si parte, rivolto alla prima cosa da vedere.
- Una luce provvisoria dove vuoi portare lo sguardo.
- Percorribile dall'inizio alla fine, e provato nel visore quando tocca a te: la scala si giudica solo lì.
- Un anchor davanti a ogni enigma, a mezzo metro dai muri; ogni enigma dentro 2 × 2 m e giocabile da seduti.
- **Head Blackout** sul rig del progetto, `XR Origin (progetto)`; muri spessi, niente indizi dietro pareti sottili.
- Ringhiere dove si può cadere, e un pavimento sotto tutto: per lo Store nessuno deve restare bloccato fuori dalla mappa.
- I tastierini vicino ai muri: sul loro Tracked Device Graphic Raycaster accendi **Check For 3D Occlusion**, se no il raggio li clicca attraverso le pareti.
- Gli indizi non solo col colore: aggiungi una forma o una scritta.
- Una rampa invisibile blocca anche l'arco del teleport: se ci si deve arrivare col teleport, dalle una Teleportation Area.

## A fine lezione
Chiudi Unity e salva il tuo lavoro: `git salva "Lezione 04"`. Il concept in `Consegne/L04` si salva insieme al progetto.
