# KT4 Distributed Tracing

## Sta je implementirano

Implementiran je distributed tracing pomocu OpenTelemetry-ja za tok kupovine ture. Trace podaci se salju u Jaeger, koji je dodat u `docker-compose.yml` kao alat za cuvanje i vizualizaciju trace podataka.

Izmene su ogranicene na observability konfiguraciju. Poslovna logika kupovine, postojece rute i RPC metode nisu menjane.

## Pokriven tok zahteva

Pokriven je postojeci tok:

```text
api-gateway -> purchase-service -> tour-service
```

Trace se generise kada korisnik doda turu u korpu:

```text
POST /api/cart/items/{tourId}
```

Gateway prosledjuje zahtev ka purchase servisu. Purchase servis u metodi `AddToCart` poziva tour servis preko HTTP JSON-RPC poziva `GetTourForPurchase`, pa isti zahtev prolazi kroz sva tri servisa.

## Pokretanje preko Docker Compose

Iz root direktorijuma projekta pokrenuti:

```powershell
docker compose up --build
```

Za build proveru servisa koji ucestvuju u ovom trace-u:

```powershell
docker compose build api-gateway purchase-service tour-service
```

## Jaeger UI

Jaeger UI je dostupan na:

```text
http://localhost:16686
```

Jaeger prima OpenTelemetry podatke preko OTLP porta `4317` unutar Docker mreze.

## Endpoint za generisanje trace-a

Potrebno je imati validan JWT token i ID objavljene ture.

Primer:

```powershell
curl -X POST "http://localhost:8000/api/cart/items/{PUBLISHED_TOUR_ID}" -H "Authorization: Bearer {JWT_TOKEN}"
```

Umesto `{PUBLISHED_TOUR_ID}` treba uneti ID objavljene ture, a umesto `{JWT_TOKEN}` validan token prijavljenog korisnika.

## Kako pronaci trace u Jaeger-u

1. Otvoriti `http://localhost:16686`.
2. U padajucem meniju `Service` izabrati `api-gateway`.
3. Kliknuti `Find Traces`.
4. Otvoriti najnoviji trace.
5. Proveriti da trace sadrzi spanove za:

```text
api-gateway
  purchase-service
    tour-service
```

Sva tri servisa treba da budu deo istog trace-a i da dele isti `traceId`.

## Pojmovi

- Trace: jedan korisnicki zahtev pracen kroz vise servisa.
- Span: jedan korak unutar trace-a, na primer obrada zahteva u jednom servisu ili HTTP poziv ka drugom servisu.
- traceId: jedinstveni identifikator celog trace-a.
- OpenTelemetry: standard i skup alata za prikupljanje telemetry podataka kao sto su tracing, metrics i logs.
- Jaeger: sistem za cuvanje, pretragu i vizualizaciju trace podataka.

## Izmenjeni fajlovi

- `docker-compose.yml`: dodat Jaeger servis i OTLP endpoint konfiguracija za servise koji ucestvuju u trace-u.
- `api-gateway/main.go`: dodata inicijalizacija OpenTelemetry tracing-a i instrumentacija inbound HTTP zahteva.
- `api-gateway/router/proxy.go`: dodat instrumentisani HTTP transport za outbound proxy pozive.
- `api-gateway/tracing/tracing.go`: dodat mali helper za OpenTelemetry tracer provider i propagaciju trace context-a.
- `api-gateway/go.mod`: dodate OpenTelemetry Go zavisnosti.
- `purchase-service/Program.cs`: dodata OpenTelemetry ASP.NET Core i HttpClient instrumentacija.
- `purchase-service/purchase-service.csproj`: dodati OpenTelemetry NuGet paketi.
- `tour-service/Program.cs`: dodata OpenTelemetry ASP.NET Core i HttpClient instrumentacija.
- `tour-service/tour-service.csproj`: dodati OpenTelemetry NuGet paketi.
- `KT4_TRACING_README.md`: dokumentacija za KT4 tracing.

## Kratko za odbranu

Implementirao sam distributed tracing pomocu OpenTelemetry-ja. Trace predstavlja jedan korisnicki zahtev kroz vise servisa, a span jedan korak u tom zahtevu. TraceId je jedinstveni identifikator celog zahteva. OpenTelemetry prikuplja i propagira tracing podatke kroz HTTP headere, a Jaeger ih cuva i vizualizuje. U Jaeger-u se vidi prolazak zahteva kroz gateway, purchase servis i tour servis u okviru istog TraceId-a.
