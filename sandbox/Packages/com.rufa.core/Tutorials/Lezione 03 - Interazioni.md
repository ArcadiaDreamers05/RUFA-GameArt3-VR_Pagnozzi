Tutorial della lezione 3, letto da RUFA > Tutorial e da tools/stima-tempi.py. Sintassi: "# " capitolo ("# Da soli ..." = lista di compiti, in qualsiasi ordine; un compito "## Extra..." è un punto in più, fuori dal punteggio), "## " pagina; righe "> evidenzia: Finestra | Etichetta | Oggetto (facoltativo: il campo si evidenzia solo su quell'oggetto)", "> verifica: id arg | arg", "> azione: Testo del pulsante | id | arg", "> attesa: secondi cosa", "> lavoro: minuti-lento minuti-veloce".

# 0 · Si parte

## Cosa farai in questa lezione
Oggi impari le interazioni di XR Interaction Toolkit: prendere, lanciare, premere, infilare una chiave, aprire un cassetto, tirare una leva, entrare in un trigger, usare un pannello nel mondo, accendere una torcia, e farlo anche con le mani, senza controller. Lavori nella **Sala delle interazioni**, un capannone con sei postazioni numerate. Anche qui qualcuno ha sabotato qualcosa: prima trovi i sintomi, poi capisci com'è fatta un'interazione, poi curi e costruisci.

Poi lavori da solo: una verifica, una fuga da una fila di stanze, e il tuo progetto d'esame.

## Prepara le mani
Oggi il rig ha anche le mani: il Quest le vede con le sue telecamere, senza controller. Premi il pulsante: Unity importa i sample delle mani, cioè i modelli, le gesture e il rig con le mani di XR Interaction Toolkit, e accende in OpenXR le funzioni che leggono le mani. Poi aspetta che finisca di compilare. Come il simulatore, i sample restano fuori da git.
> azione: Prepara le mani | prepara-mani
> verifica: mani-pronte
> attesa: 60 importazione

## Importa la palestra
Premi il pulsante: Unity copia la palestra in `Assets/Samples/RUFA Core/Lezione 03 - Interazioni` e apre la scena `Sala delle interazioni`. La copia è tua: `git aggiorna` non la tocca.
> azione: Importa la palestra | importa-sample | Lezione 03 - Interazioni
> verifica: sample-importato Lezione 03 - Interazioni

## Il simulatore
Le prove in Play si fanno col simulatore, come nella lezione 2: **RUFA ▸ Play mode ▸ Simulatore VR**.
> verifica: simulatore-acceso

## I tasti per le interazioni
Oltre ai tasti della lezione 2, oggi ti servono questi:
- **mouse**: il raggio della mano destra segue il cursore. Per puntare un oggetto ci porti sopra il mouse;
- **G** (grip): afferra. Finché lo tieni premuto l'oggetto resta in mano, quando lo rilasci lo lasci andare. Va bene anche il tasto sinistro del mouse. Per spostarti con un oggetto in mano tieni **G** e usa **W A S D**: con **Shift + I** la mano destra molla la presa, perché Shift passa i tasti alla mano sinistra;
- **T** (grilletto): usa l'oggetto che hai in mano, oppure preme i pulsanti dei pannelli;
- **2**: è il tasto B del controller destro. Oggi serve ad accendere la torcia;
- **]**: da lì in poi **W A S D** e **Q E** muovono solo la mano destra, per esempio per premere un pulsante col dito. Con **Tab** torni a muovere testa e mani insieme.

Attenzione a **]**: premilo una volta sola. La seconda volta il simulatore sostituisce i controller con le mani, che vedremo nel capitolo 8. Se ti capita, premi di nuovo **]** e tornano i controller.

# 1 · Il malato

## Entra in Play
Premi **Play**. La Sala è divisa in sei postazioni, una dopo l'altra dall'ingresso verso il fondo: ognuna ha un tratto di pavimento colorato e, all'inizio, un cartello «Postazione N». In questo capitolo fai il giro e guardi cosa non va: prima la stanza malata, poi le postazioni 3, 4 e 5. Non correggere niente: in Play le modifiche si perdono, e le cure arrivano nei capitoli dal 3 al 6.

Sulla destra c'è la postazione 1, la stanza malata: un tavolo con una tazza e una chiave, una porta con la serratura sull'anta.
> verifica: in-play
> attesa: 15 entrata in Play

## Sintomo 1: la tazza storta
Cammina fino al tavolo (**Shift + I**), porta il mouse sulla tazza e tieni **G**. La prendi, ma la mano la tiene per il fondo, non per il manico.
> verifica: afferrato Tazza

## Sintomo 2: la serratura che non apre
Lascia la tazza, punta la chiave e tieni **G**. Senza lasciare **G**, porta il mouse sulla serratura, sull'anta della porta, e tieni premuto **W**: avanzi, e la chiave avanza con te lungo il raggio. Quando compare la sagoma della chiave nella serratura, lascia **W** e poi **G**: la chiave si incastra, ma la porta resta chiusa. Riprendi la chiave e prova con la tazza: nella serratura entra anche lei.

## Sintomo 3: il muro invisibile
Mettiti al centro della stanza e cammina verso il muro di fondo, a destra. A un paio di metri dal muro ti fermi contro qualcosa che non si vede.

## Sintomo 4: il pannello incollato allo schermo
In alto a destra c'è una scritta: "Questo pannello dovrebbe stare sul muro". Guardati intorno: il pannello resta fermo sullo schermo. È disegnato sopra lo schermo del PC, e nel visore non si vedrebbe.

## Sintomo 5: la scatola attraversa il muro
Vai alla postazione 3 (**Shift + I**). A destra c'è una `Scatola` su un tavolino, e dietro un muro sottile. Punta la scatola e tieni **G**, avvicinala con **K**, poi tieni anche **W**: cammini verso il muro, e la scatola lo attraversa insieme a te, come se non ci fosse.
> lavoro: 2 1

## Sintomo 6: la palla cade a piombo
A sinistra c'è la `Palla`, su un piedistallo, davanti a un bersaglio rosso. Prendila con **G**, porta il mouse sul bersaglio e tieni **I** per un attimo: la palla avanza lungo il raggio. Poi lascia **G**: invece di volare verso il bersaglio, cade dritta a terra.
> lavoro: 2 1

## Sintomo 7: la tavola passa di mano
Più avanti, a destra, c'è l'`Asse`, una tavola lunga. Prendila con **G** e tienilo premuto. Premi **[**: ora il mouse punta con la mano sinistra. Porta il mouse sull'altra estremità della tavola e premi anche **Shift**, sempre con **G**: la sinistra la prende e la destra resta vuota. Una tavola così lunga si dovrebbe poter tenere con due mani. **]** e poi **Tab** per tornare.
> lavoro: 2 1

## Sintomo 8: la porta che non si apre
Vai alla postazione 4. A sinistra c'è una porta: puntala e premi **G**. Non succede niente: l'anta non si accorge del raggio.
> lavoro: 2 1

## Sintomo 9: il pulsante che non accende
A destra, sulla colonnina, c'è un pulsante, e dietro una lampada spenta. Prova a premerlo col dito: **]**, porta la mano sopra il pulsante con **W A S D** e abbassala con **Q**. Il dito lo attraversa, e la lampada resta spenta. **Tab** per tornare.
> lavoro: 2 1

## Sintomo 10: il cassetto e la leva
Più avanti, a sinistra, c'è un mobile con un cassetto. Punta la maniglia, tieni **G** e premi **S**: ti allontani, e il cassetto esce dal mobile come se non avesse guide. A destra c'è una leva: punta il pomello rosso, tieni **G** e premi **S**. Invece di piegarsi, la leva si stacca dalla base.
> lavoro: 2 1

## Sintomo 11: il pannello che non si clicca
Vai alla postazione 5. Davanti a te c'è un pannello di controllo, con un pulsante, un toggle e uno slider. Punta il pulsante «Accendi la luce» e clicca col tasto sinistro del mouse, o premi **T**: la luce della lampada a sinistra resta spenta. Il pannello non reagisce al raggio.
> lavoro: 2 1

## Esci dal Play
Hai visto undici sintomi. Nella stanza malata: la tazza storta, la serratura che non apre, il muro invisibile, il pannello incollato allo schermo. Nelle postazioni: la scatola che attraversa il muro, la palla che cade a piombo, la tavola che passa di mano, la porta che non si apre, il pulsante che non accende, il cassetto e la leva senza guida, il pannello che non si clicca. Esci dal Play: la stanza malata la curi nel capitolo 3, le postazioni nei capitoli 4, 5 e 6.
> verifica: fuori-play

# 2 · Anatomia dell'interazione

## Interactor e interactable
Ogni interazione ha due parti:
- l'**interactor** agisce: una mano, un raggio, un dito, lo sguardo;
- l'**interactable** subisce: la tazza, la chiave, un pulsante.

## L'Interaction Manager
Tra interactor e interactable c'è l'**XR Interaction Manager**. Ce n'è uno solo per scena e fa da arbitro: a ogni fotogramma guarda tutti gli interactor e tutti gli interactable e decide chi tocca, chi prende e chi preme cosa.

Nella Hierarchy della Sala non c'è finché non premi Play. Lo aggiunge all'avvio il `Rig Bootstrap`, l'oggetto che mette in scena il rig. Se il Rig Bootstrap non ci fosse, lo creerebbe da solo XR Interaction Toolkit, il package di Unity per le interazioni: quando un interactor si accende e non trova un manager nella scena, ne aggiunge uno. In Play lo vedi nella Hierarchy col nome `XR Interaction Manager`.

L'ordine in cui il manager aggiorna interactor e interactable a ogni fotogramma si chiama **interaction update loop**. Per ora basta sapere che esiste.

## Stati ed eventi
Un interactable passa per tre stati:
- **Hover**: un interactor lo punta o lo tocca;
- **Select**: lo prendi col grip, o lo premi;
- **Activate**: mentre lo tieni, premi il grilletto. Una torcia si accende, una pistola spara.

A ogni cambio di stato l'interactable lancia un **evento**: Hover Entered, Select Entered, Activated e così via. Nell'Inspector, in **Interactable Events**, ogni evento ha una lista di chiamate, come l'On Click di un pulsante: colleghi un oggetto e scegli il metodo da chiamare. Così le cose succedono senza scrivere codice. Il componente **Simple Audio Feedback** usa proprio questi eventi: suona quando prendi o lasci un oggetto.

Anche l'interactor ha i suoi eventi, in **Interactor Events**: scattano dalla sua parte, quando lui punta, prende o lascia qualcosa. La serratura che curerai nel capitolo 3 è un interactor, e il suo script **Lock** ascolta proprio il suo Select Entered: quando la serratura prende la chiave, Lock lancia a sua volta On Unlocked.

## Gli interactor del rig
Nella cartella della palestra apri il prefab `XR Origin (sala)`. Sotto `Camera Offset`, ognuno dei due controller ha tre interactor:
- **Poke Interactor**: il dito che preme;
- **Near-Far Interactor**: la mano che prende da vicino e, col raggio, da lontano;
- **Teleport Interactor**: l'arco della lezione 2.

`Gaze Interactor`, accanto ai controller, è lo sguardo: chi non può usare le mani seleziona guardando. Con l'**XR Gaze Assistance** lo sguardo aiuta anche il raggio della mano a mirare.

Gli oggetti `Stabilized`, come `Gaze Stabilized`, hanno un **XR Transform Stabilizer**, che toglie al raggio il tremolio della testa e della mano. Accanto ai controller ci sono `Left Hand` e `Right Hand`, le mani tracciate, col Poke e il Near-Far dei controller: le usi nel capitolo 8. Sulla radice, l'**XR Input Modality Manager** passa dai controller alle mani quando posi i controller, e torna ai controller quando li riprendi. Nei rig più vecchi c'è anche l'**XR Direct Interactor**: prende solo da vicino, cosa che il Near-Far fa già.
> verifica: prefab-aperto XR Origin (sala)

## Near-Far e l'Attach Controller
Seleziona `Right Controller/Near-Far Interactor`. Il **Near-Far Interactor** cerca gli oggetti in due modi: da vicino con una sfera intorno alla mano, da lontano con un raggio curvo, disegnato dal **Curve Visual Controller**.

L'**Interaction Attach Controller**, sullo stesso oggetto, guida l'oggetto preso da lontano: lo tiene in fondo al raggio e lo sposta quando muovi la mano. Con la levetta lo avvicini o lo allontani lungo il raggio.
> verifica: selezionato Near-Far Interactor

## L'Interaction Group
Seleziona `Right Controller`: l'**XR Interaction Group** riunisce gli interactor di una mano. Quando più di uno potrebbe agire, decide chi vince: da vicino il dito ha la precedenza sul raggio. Basta saperlo: oggi non lo tocchiamo.

## Il Debugger
Esci dal prefab e apri **Window ▸ Analysis ▸ XR Interaction Debugger**: mostra in tempo reale gli interactor, gli interactable e i loro stati. Agganciala accanto alla Game view.
> verifica: finestra-aperta XR Interaction Debugger

## Guarda gli stati in Play
Premi Play e vai alla postazione 2: sul tavolo a sinistra ci sono un cubo azzurro e una lampada. Nel Debugger, scheda **Interactables**, cerca `Cubo`. Punta il cubo col mouse: è in Hover; tieni **G**: è in Select. Il cubo si colora grazie all'**XR Tint Interactable Visual**.

Poi la lampada: ha un **XR Simple Interactable**, un interactable che non si sposta ma si seleziona e si attiva. Tieni **G** sulla lampada e premi **T**: la luce resta accesa finché tieni il grilletto. Sono i suoi eventi Activated e Deactivated, collegati alla lampadina.
> verifica: afferrato Cubo
> attesa: 15 entrata in Play
> lavoro: 4 2

# 3 · La cura

## L'attach point
Esci dal Play. Seleziona `Tazza`: nel suo **XR Grab Interactable** il campo **Attach Transform** è vuoto, e la mano prende la tazza dal suo pivot, sul fondo.

Clic destro su `Tazza` (la radice, non `Manico`) ▸ **Create Empty**: nasce un figlio della tazza, accanto a `Corpo` e `Manico`. Chiamalo `Attach` e spostalo sul manico con **Position** 0,065 · 0,05 · 0: sta dove sta il manico, ma non ne è figlio. Poi trascinalo in **Attach Transform** e salva la scena. La mano terrà la tazza in quel punto, girata come l'Attach.
> evidenzia: Inspector | Attach Transform | Tazza
> verifica: scena-riferimento Tazza | XRGrabInteractable | m_AttachTransform
> lavoro: 2 1

## Il dynamic attach
In alternativa all'attach point c'è **Use Dynamic Attach**: la mano prende l'oggetto nel punto esatto in cui lo tocca. Provalo sull'`Asse`, la tavola lunga della postazione 3. Premi Play, punta la tavola vicino a un'estremità e tieni **G**: la tavola scatta di lato e ti mette in mano il suo centro, perché senza attach point la mano la prende dal pivot.

Esci dal Play, seleziona `Asse` e nel suo XR Grab Interactable accendi **Use Dynamic Attach**. Salva e riprova: la tavola resta ferma e la tieni proprio dove l'hai presa. Il dynamic attach va bene per una tavola o una pietra, che si prendono in qualsiasi punto; per una tazza o una torcia, che si tengono in un modo solo, serve l'attach point.
> evidenzia: Inspector | Use Dynamic Attach | Asse
> verifica: scena-vero Asse | XRGrabInteractable | m_UseDynamicAttach | sì
> attesa: 30 entrata in Play
> lavoro: 3 2

## Gli interaction layer
Seleziona `Serratura`, sull'anta della `Porta della stanza`. Ha un **XR Socket Interactor**: un interactor che non è una mano, ma un posto dove un oggetto si incastra. Il suo **Interaction Layer Mask** è `Default`, quindi accetta qualunque oggetto, anche la tazza. Lascia solo `Chiave` e salva.

Come nella lezione 2, un interactor vede solo gli interactable con un layer in comune. La chiave ha `Default` e `Chiave`: la mano la prende con `Default`, la serratura la riconosce da `Chiave`.
> evidenzia: Inspector | Interaction Layer Mask | Serratura
> verifica: scena-numero Serratura | XRSocketInteractor | m_InteractionLayers.m_Bits | 2 | 2
> lavoro: 1 1

## L'evento della serratura
Sulla `Serratura` c'è anche lo script **Lock**: quando il socket prende la chiave, lancia l'evento **On Unlocked**, una volta sola. L'evento è vuoto, e nessuno apre la porta.

In **On Unlocked** premi **+**, trascina `Porta della stanza` nel campo dell'oggetto e scegli **Door ▸ Open ()**. Salva.
> verifica: evento Serratura | Lock | onUnlocked | Open
> lavoro: 2 1

## Prova la porta
Premi Play e infila la chiave come nel sintomo 2. Quando lasci **G**, la chiave resta nella serratura e la porta si apre.
> verifica: stato Porta della stanza | Door | IsOpen | sì
> attesa: 15 entrata in Play
> lavoro: 2 1

## Il trigger
Esci dal Play e seleziona `Trigger del cartello`: ha un **Box Collider** e lo script **Narrative Trigger**, che scrive una frase sul pannello quando entri. Nel collider **Is Trigger** è spento: il volume è un muro solido. Accendilo e salva.
> evidenzia: Inspector | Is Trigger | Trigger del cartello
> verifica: scena-vero Trigger del cartello | BoxCollider | m_IsTrigger | sì
> lavoro: 1 1

## Chi attiva un trigger
Un trigger si accorge di un oggetto solo se uno dei due ha un **Rigidbody** o un **Character Controller**. Il rig XR ha il Character Controller sulla radice, il corpo della lezione 2, e il Narrative Trigger riconosce il giocatore proprio da lì. Un cubo senza Rigidbody spinto dentro il volume non fa scattare niente.

## Il pannello nel mondo
Seleziona `Pannello del cartello`, figlio di `Supporto del cartello`. Nella **Canvas**, **Render Mode** è **Screen Space - Overlay**: disegnato sopra lo schermo, nel visore sparisce. Mettilo su **World Space**.

La canvas tiene la misura dello schermo: è larga quasi due chilometri. Nel **Rect Transform** metti **Pos X**, **Y** e **Z** a 0, **Rotation** a 0 su X, Y e Z, **Width** 600, **Height** 400 e **Scale** 0,001 su X, Y e Z. In World Space la canvas conta in metri: con 0,001 un'unità della UI è un millimetro, e il pannello misura 60 × 40 cm. Posto e rotazione glieli dà il supporto. Salva.
> evidenzia: Inspector | Render Mode | Pannello del cartello
> verifica: scena-numero Pannello del cartello | Canvas | m_RenderMode | 2 | 2
> lavoro: 3 2

## Prova il trigger
Premi Play e cammina verso il muro di fondo: entri nel trigger e sul pannello, ora appeso al muro, compare la frase. Dopo qualche secondo sparisce: lo decide **Show Seconds** del Narrative Trigger, che può anche far partire un suono con una **Audio Source**. L'audio lo vediamo alla lezione 12.
> verifica: stato Trigger del cartello | NarrativeTrigger | Fired | sì
> attesa: 15 entrata in Play
> lavoro: 1 1

# 4 · Prendere e lanciare

## Il Movement Type
Sintomo 5, la scatola che attraversa il muro. Esci dal Play e vai alla postazione 3. Seleziona `Scatola`: nel suo XR Grab Interactable **Movement Type** è **Instantaneous**, la scatola segue la mano di colpo a ogni fotogramma, senza fisica, e per questo attraversa il `Muro sottile`. **Kinematic** la muove in modo più morbido, ma il muro non la ferma. Con **Velocity Tracking** si muove con la fisica, con una velocità, e il muro la ferma. Scegli Velocity Tracking, salva e prova: spingi la scatola contro il muro.
> evidenzia: Inspector | Movement Type | Scatola
> verifica: scena-numero Scatola | XRGrabInteractable | m_MovementType | 0 | 0
> lavoro: 2 1

## Il lancio
Sintomo 6, la palla che cade a piombo. Esci dal Play e seleziona `Palla`: è Instantaneous e **Throw On Detach** è spento, così quando la lasci cade a piombo. Mettila su **Velocity Tracking** e accendi **Throw On Detach**: quando lasci la presa, la palla parte con la velocità della mano. Salva.
> evidenzia: Inspector | Throw On Detach | Palla
> verifica: scena-vero Palla | XRGrabInteractable | m_ThrowOnDetach | sì
> lavoro: 1 1

## Colpisci il bersaglio
Premi Play. Col visore si lancia e basta. Col simulatore la mano è troppo lenta, quindi si spinge la palla lungo il raggio:
- punta la palla e tieni **G**;
- porta il mouse sul bersaglio rosso: la palla segue il raggio;
- tieni **I**: la levetta destra spinge la palla lungo il raggio;
- lascia **G** un attimo prima che tocchi il bersaglio.

Se la tocca mentre la tieni non vale: il bersaglio conta solo un oggetto lanciato. Se cade, riprendila col raggio.
> verifica: riuscito Bersaglio
> attesa: 15 entrata in Play
> lavoro: 4 2

## Due mani
Sintomo 7, la tavola che passa di mano. Esci dal Play e seleziona `Asse`, la tavola lunga. Nel suo XR Grab Interactable, **Select Mode** è **Single**: la può tenere una mano sola, e se la prende la seconda, la prima la perde. Mettilo su **Multiple** e salva.

In Play prendila con **G** e tienilo premuto, poi premi **[**: ora il mouse punta con la mano sinistra. Porta il mouse sull'altra estremità e premi anche **Shift**, sempre con **G**: adesso la tengono tutte e due le mani. **]** e poi **Tab** per tornare.

Pensa anche al caso opposto: le linee guida di Meta consigliano che un gioco si possa giocare anche con una mano sola, per chi ne ha una o non può usarle entrambe. Nel tuo progetto, quindi, usa le due mani come una possibilità in più, mai come l'unico modo per fare qualcosa.
> evidenzia: Inspector | Select Mode | Asse
> verifica: scena-numero Asse | XRGrabInteractable | m_SelectMode | 1 | 1
> lavoro: 1 1

## Da lontano
Qui non c'è niente da curare: la `Bottiglia` serve a provare la presa da lontano. Premi Play: la bottiglia è sullo scaffale, oltre la ringhiera, e non serve andarci. Resta davanti alla ringhiera, puntala e tieni **G**: il raggio del Near-Far la prende da lontano. Con **I** e **K** la allontani e la avvicini lungo il raggio.

Un oggetto piccolo, come una moneta, col raggio si prende a fatica. Nel tuo progetto puoi aggiungergli un **XR Interactable Snap Volume**, che allarga la zona in cui il raggio lo aggancia.
> verifica: afferrato Bottiglia
> lavoro: 1 1

# 5 · Pulsanti, leve e porte

## Una porta da cliccare
Sintomo 8, la porta che non si apre. Esci dal Play e vai alla postazione 4: a sinistra c'è la `Porta dei pulsanti`.

Una porta che si apre col raggio ha bisogno di due pezzi:
- lo script **Door** sulla radice `Porta dei pulsanti`: sa ruotare l'anta e, con **Open On Select** acceso, la apre quando qualcuno seleziona l'anta;
- un **interactable** sull'`Anta`, perché il raggio la possa selezionare. È questo che manca: per il raggio l'anta è un muro qualsiasi.

Seleziona `Porta dei pulsanti/Anta`, il figlio e non la radice, poi **Add Component ▸ XR Simple Interactable**. Salva.
> verifica: scena-componente Porta dei pulsanti | XRSimpleInteractable
> lavoro: 1 1

## Prova la porta
Premi Play, punta l'anta e premi **G**: la porta si apre.
> verifica: stato Porta dei pulsanti | Door | IsOpen | sì
> attesa: 15 entrata in Play
> lavoro: 1 1

## Il pulsante: il dito
Sintomo 9, il pulsante che non accende. Ha due difetti: li curi uno alla volta. Esci dal Play e seleziona `Pulsante`, sulla colonnina a destra. Ha già due componenti:
- un **XR Simple Interactable**, che lo rende selezionabile;
- lo script **Push Button**: quando il pulsante viene selezionato, abbassa il cappello e lancia l'evento **On Pressed**.

Primo difetto: il dito, cioè il Poke Interactor, preme solo gli oggetti che hanno un **XR Poke Filter**, il componente che gli dice da che parte si preme. Aggiungi **XR Poke Filter** al `Pulsante`. Nel suo **Poke Configuration**, **Poke Direction** è la direzione in cui si muove il dito quando preme: di serie è **Z**, giusta per un pulsante su un pannello davanti a te. Questo pulsante invece guarda in alto e si preme dall'alto in basso: scegli **Negative Y** e salva.

Il Poke Filter è uno dei **filtri** di XR Interaction Toolkit, componenti che decidono se un'interazione può avvenire. Un altro è l'**XR Target Filter**, che sceglie quale oggetto prendere quando ce ne sono tanti vicini.
> verifica: scena-numero Pulsante | XRPokeFilter | m_PokeConfiguration.m_ConstantValue.m_PokeDirection | 5 | 5
> lavoro: 1 1

## Il pulsante: l'evento
Secondo difetto: **On Pressed** è vuoto, quindi il pulsante si abbassa ma non comanda niente. Nel Push Button, in **On Pressed** premi **+**, trascina nel campo dell'oggetto `Lampada dei pulsanti/Luce di lampada dei pulsanti`, la lampadina spenta figlia della lampada, e scegli **GameObject ▸ SetActive (bool)** con la spunta accesa. Salva.
> verifica: evento Pulsante | PushButton | onPressed | SetActive
> lavoro: 2 1

## Premi
Premi Play e premi il pulsante col dito: **]**, poi porta la mano sopra il pulsante con **W A S D** e abbassala con **Q**, finché il dito lo spinge giù. La lampada si accende. Funziona anche puntando il pulsante col raggio e premendo **G**: il Push Button reagisce a qualunque selezione. **Tab** per tornare.
> verifica: stato Lampada dei pulsanti | Light | isActiveAndEnabled | sì
> attesa: 15 entrata in Play
> lavoro: 3 2

## Il cassetto
Sintomo 10, il cassetto che esce dal mobile. Esci dal Play. Più avanti nella postazione 4, a sinistra, c'è un `Mobile`: seleziona il suo `Cassetto`.

Un cassetto vero scorre sulle guide, avanti e indietro e basta. In Unity le guide le fa un **giunto**, qui un **Configurable Joint**, che decide in quali direzioni il cassetto si può muovere:
- **X Motion** e **Y Motion** su **Locked**: niente movimenti di lato o in alto;
- **Z Motion** su **Limited**: avanti e indietro, ma solo per un tratto. **Linear Limit** è mezza corsa, 17,5 cm in avanti e indietro dal **Connected Anchor**, un punto a metà strada: chiuso il cassetto sta in fondo, aperto esce di 35 cm;
- tutte le rotazioni, **Angular X, Y e Z Motion**, su **Locked**: il cassetto non gira.

Il giunto è legato al `Mobile`, il suo **Connected Body**: il Connected Anchor è un punto del mobile, e se sposti o giri il mobile le guide vanno con lui. Per questo il mobile ha un Rigidbody con **Is Kinematic** acceso: un corpo che la fisica non muove.

Il giunto c'è già e funziona: il difetto è nella presa. L'XR Grab Interactable del cassetto è **Instantaneous**: come la scatola della postazione 3, segue la mano senza fisica, e il giunto, che è fisica, non lo può fermare. Mettilo su **Velocity Tracking** e salva: la mano spinge il cassetto con una velocità, e il giunto lo tiene sulle guide.
> evidenzia: Inspector | Movement Type | Mobile/Cassetto
> verifica: scena-numero Cassetto | XRGrabInteractable | m_MovementType | 0 | 0
> lavoro: 2 1

## La leva
Sintomo 10, la leva che si stacca. A destra, dietro il pulsante, c'è la `Leva`: aprila nella Hierarchy e seleziona il suo `Braccio`, il pezzo che si muove. La base resta ferma.

Una leva gira intorno a un perno. In Unity il perno lo fa un **Hinge Joint**, un cardine: la leva può girare solo intorno al suo asse, **X**, e con **Use Limits** acceso i **Limits** la fermano a 45° da una parte e dall'altra. Lo script **Lever** guarda l'angolo del cardine: quando tiri la leva verso di te oltre i 35° del **Pull Angle**, lancia l'evento **On Pulled**, una volta sola.

Anche qui due difetti, come nel pulsante:
- la presa è **Instantaneous**, e stacca la leva dal cardine come il cassetto dal mobile: mettila su **Velocity Tracking**;
- **On Pulled** è vuoto: premi **+**, trascina `Lampada della leva/Luce di lampada della leva` e scegli **GameObject ▸ SetActive (bool)** con la spunta.

Salva. **Track Rotation** lascialo spento: così la leva la fa girare il cardine mentre la mano tira il pomello, non la rotazione del polso.
> evidenzia: Inspector | Movement Type | Leva/Braccio
> verifica: scena-numero Leva/Braccio | XRGrabInteractable | m_MovementType | 0 | 0
> lavoro: 3 2

## Prova la leva
Premi Play e vai alla postazione 4. Punta la maniglia del cassetto, tieni **G** e premi **S**: ti allontani, e il cassetto si apre fino a fine corsa. Dentro c'è un premio. Poi punta il pomello rosso della leva, tieni **G** e premi **S**: la leva si piega verso di te e la lampada accanto si accende. Spinta dall'altra parte non scatta: il Pull Angle conta in un verso solo.
> verifica: stato Lampada della leva | Light | isActiveAndEnabled | sì
> attesa: 15 entrata in Play
> lavoro: 3 2

# 6 · La UI nel mondo

## Perché il raggio non clicca
Sintomo 11, il pannello che non si clicca. Esci dal Play e vai alla postazione 5. Il `Pannello di controllo` è una canvas in World Space, ma col raggio non si clicca. Il suo **Graphic Raycaster** è quello di un gioco su schermo: sente il mouse, non i raggi dei controller. Aggiungi un **Tracked Device Graphic Raycaster** alla canvas e salva. Il Graphic Raycaster lascialo: serve al mouse, nella versione PC e nel simulatore.
> verifica: scena-componente Pannello di controllo | TrackedDeviceGraphicRaycaster
> lavoro: 1 1

## L'EventSystem
Una UI ha bisogno di un **EventSystem**, e in VR del suo **XR UI Input Module**, che porta i raggi e il dito alla UI. Nella Sala non ci sono: XR Interaction Toolkit li crea in Play, quando mancano. Nel tuo progetto puoi metterli tu, da **GameObject ▸ XR ▸ UI Event System**.

## Il pulsante
Seleziona `Pulsante luce`. In **On Click ()** premi **+**, trascina `Lampada della UI/Luce di lampada della ui` e scegli **Light ▸ enabled** con la spunta. Salva.
> verifica: evento Pulsante luce | Button | m_OnClick | set_enabled
> lavoro: 1 1

## Lo slider
Seleziona `Slider intensità`. In **On Value Changed (Single)** premi **+**, trascina la stessa lampadina e scegli **Light ▸ intensity**, in cima alla lista, tra le **Dynamic float**: così il valore dello slider passa alla luce. Salva.
> verifica: evento Slider intensità | Slider | m_OnValueChanged | set_intensity
> lavoro: 2 1

## Il toggle
Seleziona `Toggle cartello`. In **On Value Changed (Boolean)** premi **+**, trascina `Cartello nascosto` e scegli **GameObject ▸ SetActive**, tra le **Dynamic bool**. Salva.
> verifica: evento Toggle cartello | Toggle | onValueChanged | SetActive
> lavoro: 1 1

## Prova la UI
Premi Play, punta il pannello e clicca col tasto sinistro del mouse o con **T**: il pulsante accende la lampada, lo slider ne cambia l'intensità, il toggle fa comparire il cartello. Col visore si clicca anche col dito, toccando il pannello.
> attesa: 15 entrata in Play
> lavoro: 2 1

## Lazy Follow
Esci dal Play: `Cartello che segue` ancora non segue nessuno. Aggiungigli un **Lazy Follow**: in Play il cartello resta davanti al tuo sguardo e ti raggiunge con un po' di ritardo, quando ti giri. È il modo di tenere un messaggio sempre leggibile. Basta provarlo.

## Il menu sul polso
Esci dal Play e apri il prefab `XR Origin (sala)`. Dalla cartella della palestra trascina `Menu sul polso` sopra `Camera Offset` nella Hierarchy, così che diventi suo figlio: deve stare dentro `Camera Offset`, allo stesso livello di `Left Controller` e `Right Controller`. Ha un **Hand Menu** e due ancore, `Ancora sinistra` e `Ancora destra`: ognuna segue il controller del suo lato e, quando lo posi, la mano. Trascinale in **Left Palm Anchor** e **Right Palm Anchor**. Coi controller il menu compare sopra il controller sinistro. Con le mani tracciate compare quando giri il palmo sinistro verso l'alto e verso di te, come per guardarti il palmo: questo si prova solo col visore, perché il simulatore non passa al menu la rotazione della mano.

Poi apri il menu, seleziona `Pannello/Slider velocità` e in **On Value Changed (Single)** collega `Locomotion/Move` con **DynamicMoveProvider ▸ moveSpeed**, tra le Dynamic float. Salva. Se vuoi, collega anche i pulsanti `30°` e `45°`: nel loro **On Click ()**, `Locomotion/Turn` con **SnapTurnProvider ▸ turnAmount** e il valore 30 o 45.
> verifica: prefab-componente XR Origin (sala) | Camera Offset | HandMenu
> lavoro: 5 3

## Prova il menu
Premi Play: il menu è sopra la mano sinistra. Punta lo slider e clicca verso destra: la velocità sale. Cammina e senti la differenza.
> verifica: stato XR Origin (sala)(Clone)/Locomotion/Move | DynamicMoveProvider | moveSpeed | 2,6 | 10
> attesa: 15 entrata in Play
> lavoro: 1 1

# 7 · La torcia

## L'input dei controller
Nella lezione 2 hai visto le **azioni** dell'Input System: un'azione collega un comando del controller, come la levetta sinistra, a un nome, come Move. I componenti di XR Interaction Toolkit le leggono con un **input reader**. Ora ne usi uno anche tu, per accendere una torcia col tasto B.

## Le convenzioni dei tasti
Chi gioca col Quest si aspetta che i tasti facciano sempre le stesse cose, e le linee guida di Meta le mettono in fila:
- **grip**: prendere e tenere;
- **grilletto**: usare quello che hai in mano e premere i pulsanti dei pannelli;
- **levette**: muoversi, la sinistra per camminare, la destra per girarsi o teletrasportarsi;
- **A, B, X, Y**: le azioni del gioco, come saltare o accendere una torcia;
- il tasto **menu** del controller sinistro apre il menu del gioco; il tasto **Meta** del destro è del sistema, e nessun gioco lo usa.

La torcia sul tasto B le rispetta. Rispettale anche nel tuo progetto: chi prende in mano il tuo gioco non deve reimparare i tasti.

## Gli input reader
Un input reader ha un campo **Input Source Mode** che dice da dove arriva il comando:
- **Input Action Reference**: un'azione di un asset, come quelle di `XRI Default Input Actions`;
- **Input Action**: un'azione scritta nel componente stesso, con i suoi binding;
- **Manual Value**: il valore lo scrive il codice.

Per la torcia usi **Input Action**: un'azione tutta sua, senza toccare le azioni degli Starter Assets.

## Costruisci la torcia
Esci dal Play e apri il prefab `XR Origin (sala)`. Clic destro su `Camera Offset/Right Controller` ▸ **Light ▸ Spotlight**: chiamala `Torcia`, con **Position** e **Rotation** a 0, **Range** 10, **Spot Angle** 40 e **Intensity** 3. Poi spegnila, togliendo la spunta accanto al nome.

Seleziona `Right Controller` e aggiungi il componente **Button Toggle**: a ogni pressione accende o spegne il suo **Target**. Trascina `Torcia` in Target.
> verifica: prefab-componente XR Origin (sala) | Camera Offset/Right Controller | ButtonToggle
> lavoro: 3 2

## Il binding
Nel Button Toggle il campo **Button** è in modalità **Input Action** e mostra due azioni: la prima, `Accendi`, dice quando il tasto viene premuto; la seconda porta il valore, e qui non serve. La prima non ha ancora un binding, cioè un tasto. Premi il suo **+** ▸ **Add Binding**, seleziona il binding nuovo e nel campo **Path** premi **T**, per scrivere il percorso a mano: `<XRController>{RightHand}/secondaryButton`, il tasto B del controller destro. Salva.

## Nel buio
Premi Play e vai alla postazione 6, la stanza buia. Si entra dall'apertura a destra della facciata, poi il corridoio gira a sinistra e di nuovo a destra: da nessuna parte c'è una linea dritta per il sole. Dentro è buio pesto. Premi **2**: la torcia si accende, e punta col controller destro, cioè dove sta il mouse. Cerca il quadro sul muro di fondo.
> verifica: riuscito Quadro
> attesa: 15 entrata in Play
> lavoro: 3 2

# 8 · Le mani

## Le mani del Quest
Il Quest vede le mani con le sue telecamere: ne ricostruisce lo scheletro, 26 articolazioni per mano, a ogni fotogramma. In Unity le legge **XR Hands**, il package delle mani tracciate, attraverso OpenXR: è quello che hai preparato nel capitolo 0, con le funzioni **Hand Tracking Subsystem** e **Meta Hand Tracking Aim** accese. La seconda dà alle mani un raggio e il pinch, pollice e indice che si toccano, come il grilletto di un controller.

## Le mani nel rig
Esci dal Play e apri il prefab `XR Origin (sala)`. Sotto `Camera Offset`, accanto ai controller, ci sono `Left Hand` e `Right Hand`. Hanno due interactor dei controller: il **Poke Interactor** sulla punta dell'indice e il **Near-Far Interactor**, che prende col pinch, da vicino e col raggio. Il Teleport Interactor no: con le mani non ci si teletrasporta. Ogni mano ha due modelli, uno per il Quest e uno per Android XR: in Play lo **Hand Visualizer** accende quello del visore. L'XR Input Modality Manager, sulla radice, per ogni lato tiene acceso il controller o la mano, quello che il visore vede.
> verifica: prefab-aperto XR Origin (sala)

## Passa alle mani
Esci dal prefab e premi Play. Nel simulatore premi **]** due volte: la prima sceglie la mano destra, la seconda passa dai controller alle mani. Al posto dei controller compaiono due mani. Con un altro **]** tornano i controller.
> verifica: modalita-mani
> attesa: 15 entrata in Play

## Prendi e premi con le mani
Con le mani la levetta non c'è: per spostarti premi **Tab** e cammina con **W A S D**, testa e mani insieme. Vai alla postazione 2 e punta il cubo col mouse. Con le mani il clic sinistro fa il pinch: un clic e la mano prende il cubo, un altro clic e lo lascia. Poi cammina fino al pulsante della postazione 4, premi **]** per muovere solo la mano destra e **N** per puntare l'indice: con **W A S D** e **Q** portala sopra il pulsante e abbassala, come col controller. Il simulatore ha tre pose della mano: **N** l'indice, **M** il pinch, **K** la presa.
> lavoro: 3 2

## La gesture
Esci dal Play. All'ingresso della Sala, a sinistra, c'è la `Lampada delle mani`, sotto il cartello «La mano». Una **gesture** è una forma della mano che vuol dire qualcosa: il pollice su, il pugno, la mano aperta. Seleziona la lampada e aggiungi **Hand Gesture**, lo script del corso: non **Static Hand Gesture**, del sample, che vuole altri riferimenti. Riconosce una gesture tenuta per un attimo, l'**Hold Time**, e lancia **On Performed**.

In **Shape Or Pose** trascina `Thumbs Up`, dal sample delle gesture: `Assets/Samples/XR Hands/1.8.1/Gestures/Examples/Hand Poses`. È una **posa**: una **forma**, il pugno col pollice dritto, più un orientamento, il pollice verso l'alto. Le forme stanno nella cartella accanto, `Hand Shapes`. Lascia **Hand** su **Right** e salva.
> evidenzia: Inspector | Shape Or Pose | Lampada delle mani
> verifica: scena-riferimento Lampada delle mani | HandGesture | shapeOrPose
> lavoro: 2 1

## L'evento della gesture
In **On Performed** premi **+**, trascina `Lampada delle mani/Luce di lampada delle mani` e scegli **GameObject ▸ SetActive (bool)** con la spunta. Salva.
> verifica: evento Lampada delle mani | HandGesture | onPerformed | SetActive
> lavoro: 1 1

## Prova la gesture
Il simulatore il pollice su non lo sa fare: ha solo le tre pose di prima. La gesture si prova col visore. Con la Sala aperta, **RUFA ▸ Build ▸ Scena aperta (APK)** ne fa un'app a parte, `Builds/Quest/RUFA_Scena.apk`: installala con `adb install -r`, come nella lezione 1. Nel visore posa i controller, chiudi il pugno e alza il pollice: la lampada si accende. Nella fuga c'è un Extra con la stessa gesture.

## Le microgesture
Sul Quest ci sono anche le **microgesture**: piccoli gesti del pollice sul lato dell'indice, uno sfioramento a destra, a sinistra, avanti o indietro, oppure un tocco. Servono a muoversi o a scegliere senza alzare la mano. Sono del **Meta XR Core SDK**, che il corso non usa: basta sapere che esistono.

## Mani o controller
Le mani sono naturali per prendere da vicino, premere un pulsante, toccare un pannello. Ma non hanno tasti né vibrazione, si stancano a stare alzate, e il visore le perde quando escono dal campo delle telecamere o una copre l'altra. I controller sono più precisi per mirare e per muoversi. Le linee guida di Meta consigliano di pensare il gioco per tutti e due quando si può: il rig passa da sé dagli uni alle altre, a te resta scegliere interazioni che funzionino con entrambi. Nella Sala, per esempio, il teletrasporto e la camminata con la levetta ci sono solo coi controller, mentre il menu sul polso, con le due ancore del capitolo 6, segue anche le mani e si apre col palmo rivolto verso di te. Il rig del tuo progetto, `XR Origin (progetto)`, ha solo i controller: le mani le provi qui, nella Sala.

# Da soli · Verifica

## La fuga
Ora tocca a te, senza istruzioni passo passo. Premi il pulsante: si apre la scena `Verifica`, una fila di stanze dal punto A al punto B. Ogni porta si apre solo con qualcosa di questa lezione.

Le regole:
- l'**ambiente non si sposta**: niente oggetti spostati, ruotati o scalati;
- puoi **aggiungere** componenti e oggetti, e collegare eventi;
- i pezzi delle stanze ci sono tutti, ma **rotti o senza componenti**, come nella Sala: il cassetto che la mano non prende, la chiave che non si prende, la serratura e il pulsante che hanno solo la forma, il trigger che fa da muro, il tastierino che il raggio non clicca, la palla che non vola;
- il rig della fuga, `XR Origin (fuga)`, non ha né la torcia né il menu: aggiungili tu;
- le porte si aprono solo quando qualcosa le apre, mai cliccando l'anta.

Qui sotto c'è cosa devi ottenere, non come; se ti blocchi, sotto ogni compito c'è **Mostra la soluzione**: il compito conta lo stesso, ma resta segnato. I compiti si spuntano da soli mentre lavori, e in alto vedi il punteggio. Le prove in Play si fanno col **Simulatore VR** acceso (RUFA ▸ Play mode ▸ Simulatore VR), oppure col visore e Quest Link (RUFA ▸ Play mode ▸ VR con Quest Link). Quello che ottieni in Play resta spuntato. È un'autovalutazione: serve a te.
> azione: Apri la verifica | apri-scena | Lezione 03 - Interazioni | Verifica

## Il cassetto
Nella stanza 1, a sinistra, c'è il `Mobile 1`, e nel suo `Cassetto 1` c'è la chiave. Il cassetto ha già il suo Configurable Joint, come quello della Sala, ma la mano non lo prende.

**Risultato atteso:** in Play apri il cassetto fino in fondo.
> verifica: stato Cassetto 1 | Slide | Value | 0,95 | 1
> soluzione: Nella Hierarchy apri `Ambiente` ▸ `Mobile 1` e seleziona `Cassetto 1`.
> soluzione: **Add Component** ▸ **XR Grab Interactable**.
> soluzione: **Movement Type** su **Velocity Tracking**, e togli la spunta a **Track Rotation**: il cassetto lo guida il giunto, non il polso.
> soluzione: In **Attach Transform** trascina il figlio `Attach` del cassetto, sulla maniglia. Salva.
> soluzione: In Play punta la maniglia, tieni **G** e premi **S**: ti allontani, e il cassetto si apre fino in fondo.
> lavoro: 3 2

## La presa del cassetto
**Risultato atteso:** il cassetto si prende dalla maniglia, il suo `Attach`, e si muove con la fisica, così il giunto lo tiene sulle guide.
> verifica: scena-numero Cassetto 1 | XRGrabInteractable | m_MovementType | 0 | 0

## La chiave
La chiave è nel cassetto, ma non si prende. Il suo `Attach` è l'anello, da dove la mano la tiene. Col simulatore mettiti sopra il cassetto aperto e guarda in basso con la **freccia giù**: da lontano il raggio prende il bordo del cassetto.

**Risultato atteso:** in Play prendi la chiave in mano.
> verifica: afferrato Chiave
> soluzione: Seleziona `Chiave`, in `Ambiente`, e aggiungi un **XR Grab Interactable**.
> soluzione: In **Attach Transform** trascina il suo figlio `Attach`, l'anello.
> soluzione: In **Interaction Layer Mask** spunta **Default** e **Chiave**: così entra anche in una serratura sul layer Chiave. Salva.
> soluzione: In Play apri il cassetto, mettiti sopra il cassetto aperto, guarda in basso con la **freccia giù**, punta la chiave e tieni **G**.
> lavoro: 2 1

## La serratura
Sull'anta della porta 1, sotto il pomello, c'è la `Serratura`: ha solo la forma, senza socket e senza script, e il suo `Attach` è il punto dove la chiave si ferma, con la punta dentro l'anta. Ci deve entrare la chiave, e quando entra la porta si apre, come nella stanza malata della Sala. Chiave e serratura si trovano solo se hanno un interaction layer in comune: se metti la serratura sul layer `Chiave`, come nella Sala, anche la chiave lo deve avere. Col simulatore tieni **G** sulla chiave, avvicinati con **W A S D**, punta la serratura e lascia **G**.

**Risultato atteso:** in Play metti la chiave nella serratura, e la porta 1 si apre.
> verifica: stato Porta 1 | Door | IsOpen | sì
> soluzione: Seleziona `Serratura`: in `Ambiente` ▸ `Porta 1` ▸ `Anta`.
> soluzione: **Add Component** ▸ **XR Socket Interactor**, e in **Attach Transform** trascina il figlio `Attach` della serratura. In **Interaction Layer Mask** lascia **Everything**, oppure solo **Chiave** se la chiave ha il layer Chiave.
> soluzione: **Add Component** ▸ **Lock**.
> soluzione: In **On Unlocked ()** premi **+**, trascina `Porta 1` e scegli **Door ▸ Open ()**. Salva.
> soluzione: In Play apri il cassetto e prendi la chiave; tenendo **G** avvicinati alla porta con **W A S D**, punta la serratura e lascia **G**.
> lavoro: 6 2

## La serratura collegata
**Risultato atteso:** la serratura, quando la chiave entra, apre la porta 1.
> verifica: evento Serratura | Lock | onUnlocked | Open

## Il pulsante
Nella stanza 2, sulla colonnina a sinistra, c'è il `Pulsante 2`: ha solo la forma, la base e il `Cappello`, senza componenti. Come il pulsante della Sala, va premuto, e premuto apre la porta 2. Col simulatore si preme col raggio e **G**.

**Risultato atteso:** in Play premi il pulsante, e la porta 2 si apre.
> verifica: stato Porta 2 | Door | IsOpen | sì
> soluzione: Seleziona `Pulsante 2`, in `Ambiente`.
> soluzione: **Add Component** ▸ **Push Button**: aggiunge da solo l'XR Simple Interactable.
> soluzione: In **Cap** trascina il suo figlio `Cappello`.
> soluzione: In **On Pressed ()** premi **+**, trascina `Porta 2` e scegli **Door ▸ Open ()**. Salva.
> soluzione: In Play punta il pulsante col raggio e premi **G**.
> lavoro: 6 3

## Il pulsante collegato
**Risultato atteso:** il pulsante, quando lo premi, apre la porta 2.
> verifica: evento Pulsante 2 | PushButton | onPressed | Open

## Entra nel buio
Oltre la porta 2 c'è la stanza buia. L'ingresso gira due volte, così il sole non entra, e dove l'ingresso incontra il corridoio il `Trigger della stanza buia` fa da muro, come il trigger del cartello nella Sala. Quando lo attraversi, il cartello sul muro davanti ti dice cosa cercare.

**Risultato atteso:** in Play entri nel corridoio, e il trigger scatta.
> verifica: stato Trigger della stanza buia | NarrativeTrigger | Fired | sì
> soluzione: Seleziona `Trigger della stanza buia`: in `Ambiente` ▸ `Stanza buia`.
> soluzione: Nel suo **Box Collider** spunta **Is Trigger**. Salva.
> soluzione: In Play attraversa la porta 2 e cammina nell'ingresso fino al corridoio: il cartello sul muro scrive «Il codice è sul muro. Ti serve una luce.».
> lavoro: 3 1

## Il trigger
**Risultato atteso:** il trigger della stanza buia si attraversa.
> verifica: scena-vero Trigger della stanza buia | BoxCollider | m_IsTrigger | sì

## La torcia nel rig
Il rig della fuga non ha la torcia: costruiscila come nel capitolo 7, sul `Right Controller` del prefab `XR Origin (fuga)`.

**Risultato atteso:** il rig della fuga ha una torcia che si accende col tasto B.
> verifica: prefab-componente XR Origin (fuga) | Camera Offset/Right Controller | ButtonToggle
> soluzione: Nella cartella della palestra apri il prefab `XR Origin (fuga)` con un doppio clic.
> soluzione: Clic destro su `Camera Offset/Right Controller` ▸ **Light ▸ Spotlight**: chiamala `Torcia`, **Position** e **Rotation** a 0, **Range** 10, **Spot Angle** 40, **Intensity** 3, e togli la spunta accanto al nome.
> soluzione: Seleziona `Right Controller`, aggiungi **Button Toggle** e trascina `Torcia` in **Target**.
> soluzione: Nel campo **Button**, sulla prima azione, `Accendi`, premi **+** ▸ **Add Binding**; seleziona il binding nuovo e nel campo **Path** premi **T** e scrivi `<XRController>{RightHand}/secondaryButton`.
> soluzione: Salva il prefab.
> lavoro: 6 3

## Il codice
In fondo alla stanza buia, sul muro, è appeso il `Codice`: al buio non si legge. Col simulatore la torcia si accende col tasto **2**: punta il mouse sul muro di fondo. Il codice ti serve nella stanza 4.

**Risultato atteso:** in Play la torcia illumina il codice.
> verifica: riuscito Codice
> soluzione: In Play entra nella stanza buia e vai in fondo, davanti al muro.
> soluzione: Premi **2**: la torcia si accende.
> soluzione: Punta il mouse sul quadro del muro di fondo: il codice compare nella luce. Segnatelo.
> lavoro: 3 1

## Il tastierino
L'uscita della stanza buia porta alla stanza 4. A sinistra della porta 4 c'è il `Tastierino`: il suo Keypad è già collegato alla porta, e il codice giusto la apre. Ma il raggio non lo clicca, come il pannello della postazione 5 della Sala. Col simulatore i tasti si premono col raggio e **T**, o col tasto sinistro del mouse.

**Risultato atteso:** in Play scrivi il codice, e la porta 4 si apre.
> verifica: stato Porta 4 | Door | IsOpen | sì
> soluzione: Seleziona `Tastierino`, in `Ambiente`.
> soluzione: **Add Component** ▸ **Tracked Device Graphic Raycaster**. Il Graphic Raycaster lascialo. Salva.
> soluzione: In Play punta un tasto alla volta col raggio e premi **T**: scrivi il codice della stanza buia.
> lavoro: 4 2

## Il tastierino per i raggi
**Risultato atteso:** il tastierino si clicca coi raggi dei controller.
> verifica: scena-componente Tastierino | TrackedDeviceGraphicRaycaster

## Il lancio
Nella stanza 5 la `Palla` è sul piedistallo, e 3 m più avanti pende la `Campana`: colpita, apre la porta 5. La palla però, quando la lasci, cade a piombo, come quella della Sala. Col simulatore si lancia come nella Sala: punta la palla e tieni **G**, porta il mouse sulla campana, tieni **I** finché la palla è quasi arrivata, poi lascia **G**.

**Risultato atteso:** in Play colpisci la campana con la palla.
> verifica: riuscito Campana
> soluzione: Seleziona `Palla`, in `Ambiente`.
> soluzione: Nel suo **XR Grab Interactable**: **Movement Type** su **Velocity Tracking**, e spunta **Throw On Detach**. Salva.
> soluzione: In Play punta la palla e tieni **G**, porta il mouse sulla campana, tieni **I** finché la palla è quasi arrivata, poi lascia **G**.
> lavoro: 6 3

## La palla che vola
**Risultato atteso:** la palla, quando la lasci, parte con la velocità della mano.
> verifica: scena-vero Palla | XRGrabInteractable | m_ThrowOnDetach | sì

## L'arrivo
Oltre la porta 5 c'è il punto B.

**Risultato atteso:** in Play arrivi al punto B.
> verifica: sopra-oggetto Arrivo
> soluzione: In Play, aperta la porta 5, attraversala e cammina fino al pavimento della stanza B.
> lavoro: 1 1

## L'ambiente intatto
**Risultato atteso:** nessun oggetto dell'ambiente spostato, ruotato o scalato.
> verifica: ambiente-intatto Ambiente
> soluzione: Se il compito resta ✗, un oggetto di `Ambiente` è stato spostato, ruotato o scalato: **Ctrl+Z** finché torna ✓.

## Extra: il menu sul polso
Il `Menu sul polso` della cartella della palestra, nel rig della fuga, come nel capitolo 6: dentro `Camera Offset`, con le sue due ancore e lo slider collegato alla velocità. Col simulatore il menu è sopra il controller sinistro: punta la parte destra dello slider e premi **T**. È un punto in più, fuori dal punteggio.

**Risultato atteso:** in Play il menu porta la velocità sopra 2,5 m/s.
> verifica: stato XR Origin (fuga)(Clone)/Locomotion/Move | DynamicMoveProvider | moveSpeed | 2,6 | 10
> soluzione: Apri il prefab `XR Origin (fuga)`.
> soluzione: Dalla cartella della palestra trascina `Menu sul polso` sopra `Camera Offset`: diventa suo figlio, allo stesso livello di `Left Controller` e `Right Controller`.
> soluzione: Nell'**Hand Menu** del menu trascina `Ancora sinistra` in **Left Palm Anchor** e `Ancora destra` in **Right Palm Anchor**.
> soluzione: Apri il menu, seleziona `Pannello/Slider velocità`; in **On Value Changed (Single)** premi **+**, trascina `Locomotion/Move` del rig e scegli **DynamicMoveProvider ▸ moveSpeed**, tra le Dynamic float. Salva il prefab.
> soluzione: In Play il menu è sopra il controller sinistro: punta la parte destra dello slider e premi **T**.

## Extra: il pollice su
La porta 5 si apre anche col pollice su della mano destra, la gesture della Sala. Il simulatore non la sa fare: si prova col visore, con **RUFA ▸ Build ▸ Scena aperta (APK)** sulla fuga. È un punto in più, fuori dal punteggio.

**Risultato atteso:** sulla porta 5 un Hand Gesture col pollice su la apre.
> verifica: evento Porta 5 | HandGesture | onPerformed | Open
> soluzione: Seleziona `Porta 5`, in `Ambiente`, e aggiungi **Hand Gesture**.
> soluzione: In **Shape Or Pose** trascina `Thumbs Up`, da `Assets/Samples/XR Hands/1.8.1/Gestures/Examples/Hand Poses`; lascia **Hand** su **Right**.
> soluzione: In **On Performed ()** premi **+**, trascina `Porta 5` e scegli **Door ▸ Open ()**. Salva.

# Da soli · Il tuo progetto

## La tua scena
Premi il pulsante: si apre la tua scena `Progetto`, o Unity la crea se non c'è ancora. Oggi aggiungi le prime interazioni: l'oggetto da prendere, la chiave e la porta, il trigger che racconta. Puoi costruirle dai componenti, come nella Sala, o partire dai mattoncini del corso, in `Assets/RUFA/Prefabs`: chiave, serratura, porta, pulsante, leva, mobile col cassetto, manovella, manopola, slider, interruttore, tastierino, trigger narrativo e pannello, già montati con gli stessi componenti di oggi. Nella scena `Sandbox`, sotto `Mini-esempi`, li trovi collegati a qualcosa: seleziona un mattoncino e guarda il suo evento nell'Inspector. Se li usi, lavora su una copia in `Assets/Progetto`.

A fine lezione chiudi Unity e salva il tuo lavoro: `git salva "Lezione 03"`.
> azione: Crea la scena del progetto | crea-progetto
