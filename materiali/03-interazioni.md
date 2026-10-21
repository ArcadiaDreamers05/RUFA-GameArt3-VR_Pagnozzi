# Lezione 3 — Interazioni con XR Interaction Toolkit

## Cosa impari
Come funziona un'interazione in XR Interaction Toolkit 3 (interactor, interactable, Interaction Manager, stati ed eventi) e come si costruiscono quelle di un'escape room o di un walking simulator: prendere e lanciare, infilare una chiave in una serratura, premere un pulsante col dito, aprire una porta, un cassetto, tirare una leva, entrare in un trigger narrativo, usare un pannello nel mondo, accendere una torcia con un tasto del controller, aprire un menu sul polso. E come si fa tutto con le mani tracciate del Quest, gesture comprese.

## Come si parte
1. Unity chiuso, nella tua cartella del corso: `git aggiorna`.
2. Apri il progetto `sandbox/` con Unity Hub. La prima volta Unity scarica XR Hands, il package delle mani: qualche minuto.
3. **RUFA ▸ Tutorial**, poi scegli `Lezione 03 - Interazioni` dal menu in alto a sinistra della finestra.
4. Premi **Prepara le mani** e aspetta che Unity finisca di compilare.
5. Premi **Importa la palestra**: si apre la scena `Sala delle interazioni`.

Il tutorial ti guida pagina per pagina. Le pagine con **Da fare** vanno avanti solo quando il progetto è a posto. Con **A−** e **A+** cambi la dimensione del testo.

## La giornata
- **Insieme** (capitoli 0–8): lavori nella Sala delle interazioni, un capannone con sei postazioni. Nella prima, la stanza malata, trovi quattro interazioni rotte; poi capisci com'è fatta un'interazione, le curi, e costruisci prese, lanci, pulsanti, porte, un cassetto e una leva, una UI nel mondo, un menu sul polso e una torcia. Poi passi alle mani: prendi e premi senza controller, e una gesture accende una lampada.
- **Da soli · Verifica**: una fuga da una fila di stanze, dalla A alla B. In ogni stanza manca un pezzo di quello che hai imparato. I compiti si spuntano da soli mentre lavori, con il punteggio in alto: è un'autovalutazione.
- **Da soli · Il tuo progetto**: le prime interazioni nella scena del tuo progetto d'esame, coi mattoncini o dai componenti.

## Comandi del simulatore per le interazioni

| Azione | Tasti |
|---|---|
| Puntare con la mano destra | il raggio va dove porti il mouse |
| Prendere, tenere, lasciare (grip) | G tenuto, oppure il tasto sinistro del mouse tenuto; lascia per mollare |
| Attivare un oggetto in mano, cliccare un pannello (grilletto) | T, oppure il tasto sinistro del mouse sul pannello |
| Avvicinare o allontanare un oggetto preso da lontano | I e K mentre lo tieni |
| Lanciare | punta la palla e tieni G, porta il mouse sul bersaglio, tieni I e lascia G un attimo prima che la palla arrivi |
| Prendere con la mano sinistra | Shift + G; con [ il mouse punta con la sinistra |
| Muovere solo la mano destra, per premere col dito | ] una volta sola (la seconda passa alle mani), poi W A S D e Q E; Tab torna a testa e mani insieme |
| Aprire un cassetto, tirare una leva | punta la maniglia o il pomello, tieni G e premi S |
| Accendere la torcia (tasto B) | 2 |
| Camminare, girare, teleport | come nella lezione 2: Shift + I J K L, I J K L |
| Portare un oggetto preso, per esempio la chiave alla serratura | G tenuto, mouse sul punto d'arrivo e W: l'oggetto avanza con te lungo il raggio. Non Shift + I: Shift fa mollare la presa alla destra |
| Passare alle mani, e tornare ai controller | ] due volte: la prima sceglie la mano destra, la seconda passa alle mani; ] ancora per tornare |
| Con le mani: prendere e lasciare | un clic sull'oggetto, la mano fa il pinch e lo prende; un altro clic lo lascia |
| Con le mani: le pose | N l'indice (per premere), M il pinch, K la presa |
| Con le mani: camminare | Tab, poi W A S D: testa e mani insieme; ] riprende la mano destra |

Il simulatore non sa fare le gesture come il pollice su: si provano col visore.

## Comandi del Quest

| Azione | Comando |
|---|---|
| Prendere, tenere, lasciare | Grip |
| Prendere da lontano | punta col raggio, poi grip; lo stick destro avvicina o allontana |
| Lanciare | lascia il grip mentre muovi il braccio |
| Attivare un oggetto in mano, cliccare un pannello | Trigger |
| Premere un pulsante | col dito, toccandolo |
| Aprire un cassetto, tirare una leva | grip sulla maniglia o sul pomello, poi tira |
| Torcia | Tasto B |
| Menu sul polso | sta sopra il controller sinistro; con le mani tracciate, gira il palmo verso l'alto |
| Con le mani | posa i controller: pollice e indice che si toccano, il pinch, prendono e cliccano; l'indice preme |
| Gesture della lezione | pollice su con la mano destra |

Le convenzioni dei tasti di Meta, da seguire anche nel tuo progetto: il grip prende, il grilletto usa quello che hai in mano e clicca, le levette muovono, A B X Y sono le azioni del gioco, il tasto menu del controller sinistro apre il menu del gioco. Il tasto Meta è del sistema.

## Le gesture
Lo script `Hand Gesture` del corso riconosce una gesture della mano, tenuta per un attimo (Hold Time), e lancia `On Performed`. Le gesture pronte sono nel sample delle mani, `Assets/Samples/XR Hands/1.8.1/Gestures/Examples`:
- **Hand Shapes**: solo le dita, per esempio il pugno o la mano aperta;
- **Hand Poses**: le dita più l'orientamento, per esempio `Thumbs Up`, il pollice in su.

Una gesture si prova col visore: con la scena aperta, **RUFA ▸ Build ▸ Scena aperta (APK)** ne fa un'app a parte, da installare con `adb install -r Builds/Quest/RUFA_Scena.apk`. Le mani le provi nella Sala e nella fuga: il rig del tuo progetto ha solo i controller.

## I mattoncini
In `Assets/RUFA/Prefabs/` ci sono pezzi già montati, con gli stessi componenti di oggi. Nella scena `Sandbox`, sotto `Mini-esempi`, li trovi collegati a qualcosa che si vede: seleziona un mattoncino e guarda il suo evento nell'Inspector. Se ti fanno comodo per il progetto, usane una copia in `Assets/Progetto`: gli originali li usa anche il demo.

| Mattoncino | Cosa c'è dentro | Evento | Come si usa |
|---|---|---|---|
| `Afferrabile` | un cubo con Rigidbody, XR Grab Interactable e `Attach` | quelli dell'XR Grab Interactable | si prende |
| `Chiave` | XR Grab Interactable sui layer Default e Chiave | | va nella `Serratura` |
| `Serratura` | XR Socket Interactor sul layer Chiave, Lock | On Unlocked | sull'anta di una porta; On Unlocked → Door ▸ Open |
| `Porta` | l'`Anta` sul cardine, Door, XR Simple Interactable sull'`Anta` (col collider del `Pannello`) | | con Open On Select si apre col clic; spento, la apre un evento |
| `Pulsante` | XR Simple Interactable, Push Button | On Pressed | si preme col raggio; per il dito aggiungi un XR Poke Filter con Poke Direction su Negative Y |
| `Leva` | sul `Braccio` un Hinge Joint da −45° a 45°, XR Grab Interactable in Velocity Tracking, Lever | On Pulled | si tira verso di sé |
| `Mobile` | sul `Cassetto` un Configurable Joint legato al mobile, XR Grab Interactable in Velocity Tracking, Slide | On Value Changed (da 0 a 1), On End | si apre tirando la maniglia |
| `Manovella` | sulla `Ruota` un Hinge Joint senza limiti, Crank | On Value Changed (da 0 a 1), On Completed | si gira in senso orario, per i giri di Turns; all'indietro torna giù |
| `Manopola` | sul `Disco` un Hinge Joint da 0° a 170°, Knob | On Value Changed (da 0 a 1) | si gira in senso orario |
| `Slider` | sul `Cursore` un Configurable Joint lungo la guida, Slide | On Value Changed (da 0 a 1), On End | si fa scorrere |
| `Interruttore` | XR Simple Interactable, Toggle Switch | On Switched On, On Switched Off | un clic accende, un altro spegne |
| `Tastierino` | canvas in World Space coi due raycaster, Keypad, tasti da 0 a 9 | On Unlocked | il codice è nel campo Code, 1234 |
| `TriggerNarrativo` | Box Collider con Is Trigger, Narrative Trigger | On Entered | ci entri: un testo sul pannello, un audio |
| `PannelloUI` | canvas in World Space col Tracked Device Graphic Raycaster, un testo | | il pannello del trigger narrativo |
| `AudioSpaziale` | Audio Source 3D in loop | | un suono che viene da un punto |

Gli eventi coi valori da 0 a 1 si collegano a una proprietà che vuole un numero, tra le Dynamic float: l'intensità di una luce, il volume di un audio.

Due avvertenze. `Mobile` e `Slider` non si scalano: le corse dei giunti sono in metri, e un mobile più grande va costruito con pezzi più grandi. `Leva`, `Manovella` e `Manopola` vanno su qualcosa che sta fermo: il loro cardine si aggancia al punto del mondo dove sono quando parte il Play, e su un'anta che si apre resterebbero indietro.

## Sul PC, senza visore
In Play senza visore parte il rig desktop: il mirino al centro dello schermo è lo stesso tipo di interactor delle mani. Tieni premuto il tasto sinistro per prendere un oggetto, il destro per attivarlo; guardando la serratura con la chiave in mano, il mirino diventa verde e la chiave entra quando rilasci. Il mirino preme gli oggetti 3D, come porte e pulsanti, ma non i pulsanti di un pannello: per un comando da premere usa il mattoncino `Pulsante`.

## Problemi frequenti

| Sintomo | Causa | Rimedio |
|---|---|---|
| L'oggetto si prende storto | manca l'attach point | un figlio `Attach` nel punto della presa, in Attach Transform |
| Nella serratura entra di tutto | il socket ha il layer Default | Interaction Layer Mask solo su Chiave |
| La chiave entra ma la porta resta chiusa | `On Unlocked` vuoto | `On Unlocked` → la porta → `Door.Open` |
| Il trigger è un muro invisibile | Is Trigger spento | accendilo nel collider |
| Il trigger non scatta | nessuno dei due ha un Rigidbody o un Character Controller, oppure ti teletrasporti oltre il volume | il rig ha il Character Controller; fai il volume largo quanto il passaggio |
| Il pannello non si vede nel visore | Render Mode Screen Space | World Space, poi nel Rect Transform posizione, rotazione, misura e scala 0,001 |
| Il raggio non clicca il pannello | sulla canvas c'è solo il Graphic Raycaster | aggiungi un Tracked Device Graphic Raycaster |
| La scatola attraversa i muri | Movement Type Instantaneous | Velocity Tracking |
| Il cassetto esce dal mobile, la leva si stacca dal cardine | Movement Type Instantaneous: ignora il giunto | Velocity Tracking |
| Il cassetto salta in avanti quando lo prendi | la mano lo prende dal centro | in Attach Transform il suo `Attach`, sulla maniglia |
| La leva non accende niente | tirata dalla parte sbagliata, o `On Pulled` vuoto | tirala verso di te; collega `On Pulled` |
| La palla cade a piombo | Throw On Detach spento, o Instantaneous | Velocity Tracking e Throw On Detach |
| Il pulsante non si preme col dito | manca l'XR Poke Filter, o la sua Poke Direction non è quella del dito | aggiungilo; per un pulsante che guarda in su, Poke Direction su Negative Y |
| La torcia non si accende | l'azione del Button Toggle non ha binding | `<XRController>{RightHand}/secondaryButton` |
| Il menu sul polso non compare | mancano le due ancore dell'Hand Menu | `Ancora sinistra` e `Ancora destra` in Left e Right Palm Anchor |
| Il menu non segue le mani | i controller come ancore: con le mani il rig li spegne | le due ancore del menu, che seguono il controller o la mano |
| La gesture non scatta | Shape Or Pose vuoto, la mano sbagliata, o sei nel simulatore | `Thumbs Up` da Hand Poses, Hand su Right, e provala col visore |
| La gesture va in errore in Play | hai aggiunto Static Hand Gesture, quello del sample | toglilo e aggiungi Hand Gesture, lo script del corso |
| Al posto dei controller compaiono le mani | hai premuto ] due volte | ancora ] e tornano i controller |
| I valori tornano come prima | li hai cambiati in Play | cambiali a gioco fermo e salva |

## Consegna
Il lavoro, salvato a Unity chiuso con `git salva "Lezione 03"`.
