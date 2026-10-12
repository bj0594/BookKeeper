Ukesoppgave 1 (TDD) - Fra behov til REST API
Info:
I denne oppgaven skal du ta et lite system fra idé til et fungerende REST API. Du skal først bruke testdrevet utvikling (TDD) til å utforske behovet for en minimal løsning og bygge domenelogikken. Deretter skal du ta funksjonaliteten du har laget og gjøre den tilgjengelig gjennom et REST API.

Målet er å erfare hele utviklingsprosessen:
Behov -> Tester -> Domenelogikk -> API-kontrakt -> Controller -> HTTP-testing

Du skal ikke bygge et stort system. Fokuset er på å forstå hvordan TDD kan brukes både når du utvikler selve logikken og når du definerer hvordan et API skal oppføre seg.

Viktig: Det er ditt ansvar å lese gjennom hele oppgaven og sørge for at alle kravene er oppfylt før du leverer. At prosjektet fungerer, er ikke alene nok for å få innleveringen godkjent. Du må også kunne vise at du har gjennomført det oppgaven ber om.

Tips: Lag gjerne en sjekkliste mens du går grundig gjennom oppgaveteksten. Da blir det enklere å holde oversikt over hva du har gjort, og hva som eventuelt gjenstår. Før du leverer, anbefaler jeg likevel at du leser gjennom oppgaveteksten én gang til og bruker sjekklisten til å kontrollere at alle kravene er oppfylt.

Velg ett av følgende prosjekter:
Poengsystem (Nybegynnervennlig) 
XP / Level-system (Intermediate)
Bibliotekssystem (Utfordrende)
Du kan også foreslå en egen idé, men denne må godkjennes av enten faglig backend-veileder eller lokasjonsansvarlig backend-veileder.

Del 1 – Utforsk behovet for minimum viable product (MVP)
Før du begynner å kode skal du utforske hva systemet faktisk trenger.

Bruk Excalidraw eller et annet egnet verktøy til å lage en enkel skisse av en minimal versjon av systemet (MVP).
Velg en diagramtype som passer løsningen din, for eksempel flytdiagram, sekvensdiagram eller klassediagram. Diagrammet skal gjøre det tydelig hvordan hovedflyten i MVP-en fungerer.

Fokuser på:

Hvilket problem løser systemet?
Hvem er brukeren?
Hva er den viktigste flyten systemet må støtte?
Hvilke objekter trengs for denne flyten?
Hva er minimum for at systemet gir mening?
Hold MVP-en avgrenset til én hovedflyt (end-to-end).
Hovedflyten kan bestå av flere steg og operasjoner, men skal dekke én konkret brukeroppgave fra start til slutt.

Ikke prøv å designe hele systemet på forhånd. Database, autentisering, frontend og andre fremtidige funksjoner trenger ikke være med. Du kan gjerne tegne fremtidige utvidelser separat, men hold selve MVP-en liten og avgrenset.

Del 2 – Utforsk behovene gjennom TDD
Nå skal du bruke testene til å utforske hvordan systemet bør oppføre seg.

Sett opp prosjektet med et testprosjekt som bruker xUnit. Start med tester som beskriver den enkleste versjonen av MVP-en.

Eksempler:
Poengsystem
En bruker kan få poeng
En bruker starter med 0 poeng
Poengsummen øker når poeng legges til
XP / Level-system
En spiller starter på level 1
XP kan legges til
Spilleren går opp i level når nok XP er oppnådd
Bibliotekssystem
En bok kan registreres
En bok kan lånes ut
En utlånt bok kan ikke lånes ut på nytt 
Start med 4-5 tester for domenelogikken. Du står fritt til å legge til flere tester dersom du mener det er nødvendig for å dekke de viktigste reglene i MVP-en.

Ikke skriv tester for funksjonalitet som ikke er nødvendig for MVP-flyten.

Bruk TDD-syklusen
For hver funksjonalitet:

Skriv en test som feiler (Red), deretter commit.
Skriv minimumskoden som gjør testen grønn (Green), deretter commit.
Refaktorer koden (Refactor), deretter commit.
Du skal kunne vise utviklingen fra Red -> Green -> Refactor.

Del 3 – Gjør systemet tilgjengelig gjennom et API
Du har nå laget en liten domenelogikk som fungerer internt i applikasjonen. Neste steg er å gjøre funksjonaliteten tilgjengelig for andre programmer og brukere gjennom et REST API. Ta utgangspunkt i hovedflyten du allerede har skissert i del 1, og bestem hvordan funksjonaliteten skal gjøres tilgjengelig gjennom HTTP.

Lag en liten api.md eller en egen seksjon i README.md hvor du beskriver API-et ditt.

Beskriv:

Hvilke endepunkter trenger API-et?
Hvilke HTTP-metoder skal brukes?
GET
POST
PATCH
DELETE
Hvilke data skal klienten sende inn?
Hva skal API-et returnere?
Hvilke HTTP-statuskoder skal brukes?
Det holder med å skrive noe som for eksempel dette:

POST /players
Request: { "name": "Ole" }
Response: 201 Created

PATCH /players/{id}/xp
Request: { "amount": 100 }
Response: 200 OK

GET /players/{id}
Response: 200 OK

Del 4 - Definer API-kontrakten med TDD
Nå skal du bruke TDD på nytt. 
Du skal lage nye tester, denne gangen skal testene beskrive hvordan API-et skal oppføre seg.

Eksempler på ting du kan teste:

GET -> 200 OK
POST -> 201 Created
Ugyldig input -> 400 Bad Request
Ressurs finnes ikke -> 404 Not Found
Flere HTTP-statuskoder finner du her: https://www.iana.org/assignments/http-status-codesLinks to an external site. 

Testene dine skal beskrive kontrakten mellom klienten og API-et. Tenk på testen som en avtale: “Hvis klienten gjør dette, forventer vi at API-et svarer slik.”

Hvis du allerede har gjennomført Red -> Green -> Refactor-syklusen i del 2, er det valgfritt å vise denne syklusen på nytt i denne delen. Hvis du ikke fikk gjennomført syklusen i del 2, får du en ny mulighet til å gjøre det i denne delen.

Del 5 – Implementer API-et
Når API-testene er skrevet, skal du implementere API-et slik at testene blir grønne.

Lag en eller flere controllere som kobler HTTP-endepunktene til domenelogikken du laget tidligere.

Ikke bygg ny domenefunksjonalitet bare for API-et. Målet er å gjøre funksjonaliteten fra MVP-en tilgjengelig gjennom HTTP.

Når testene er grønne:

Test endepunktene manuelt ved hjelp av et verktøy som passer løsningen din, for eksempel Swagger UI, Postman, Bruno, Scalar eller curl.
Kontroller at endepunktene er tilgjengelige, og at responsene samsvarer med API-kontrakten du definerte i Del 3.
På denne måten skal du kunne verifisere systemet både gjennom automatiserte tester og gjennom faktiske HTTP-kall.

Leveranse
Du skal levere en lenke til GitHub-repository som inneholder løsningen din.

Repositoryet skal inneholde:

Excalidraw-skisse av MVP-en
api.md eller en seksjon i README.md med API-kontrakten du har definert
Tester for domenelogikken
Tester for API-kontrakten
Implementasjon av domenelogikken
Implementasjon av REST API-et
README.md med informasjon om hvordan prosjektet kjøres og testes
Du står fritt til å organisere mapper og filer slik du mener er mest hensiktsmessig. Strukturen bør likevel gjøre det tydelig hvilke deler som er domenelogikk, API, Excalidraw og tester.

Hvordan du velger å strukturere løsningen er en del av oppgaven. Tenk derfor over hvordan du kan skille ansvar og gjøre prosjektet enkelt å forstå og videreutvikle.

Veien videre
For kommende oppgaver i modulen skal dere commite til github-repoet. Dette er for å holde Githuben deres ryddig, men også for å gi dere trening i å skrive commit meldinger.