namespace QuotationAccelerator.Infrastructure.Persistence;

internal static class DefaultMailResponseSamples
{
    internal sealed record Sample(string Id, string KeywordsJson, string ReplyBody);

    internal static readonly Sample[] All =
    [
        new(
            "mail-response-quote-without-drawing",
            """["preisanfrage ohne zeichnung","angebot ohne zeichnung","kostenvoranschlag","quote without drawing"]""",
            """
            Vielen Dank für Ihre Anfrage zur Lohnfertigung.

            Für ein verbindliches Angebot benötigen wir eine technische Zeichnung (PDF bevorzugt) mit Angaben zu Material, Stückzahl und Oberflächenbehandlung. Senden Sie uns diese Unterlagen – wir melden uns zeitnah mit einem Angebot.
            """.Trim()),
        new(
            "mail-response-delivery-time",
            """["lieferzeit","delivery time","durchlaufzeit","lieferfrist","wann lieferung"]""",
            """
            Vielen Dank für Ihre Anfrage.

            Die Lieferzeit hängt von Material, Stückzahl und Fertigungsumfang ab. Mit vollständiger Zeichnung erstellen wir in der Regel innerhalb von 2–3 Werktagen ein Angebot. Expressfertigung ist auf Anfrage möglich.
            """.Trim()),
        new(
            "mail-response-material",
            """["material","werkstoff","s235","s355","edelstahl","stahlblech","aluminium"]""",
            """
            Vielen Dank für Ihre Materialanfrage.

            Wir verarbeiten gängige Baustähle (z. B. S235, S355), Edelstahl und Aluminium im Rahmen der Lohnfertigung. Bitte nennen Sie Werkstoff und Blechdicke in der Zeichnung oder E-Mail – so können wir präzise kalkulieren.
            """.Trim()),
        new(
            "mail-response-surface-treatment",
            """["verzinken","pulverbeschichtung","oberfläche","oberflächenbehandlung","surface treatment","feuerverzinkt"]""",
            """
            Vielen Dank für Ihre Nachricht.

            Wir bieten Verzinken, Pulverbeschichtung, Entgraten und weitere Oberflächenbehandlungen im Rahmen der Lohnfertigung an. Nennen Sie die gewünschte Oberfläche in der Zeichnung oder in Ihrer E-Mail.
            """.Trim()),
        new(
            "mail-response-small-batch",
            """["mindestmenge","kleinserie","stückzahl","einzelteil","prototyp","musterstück"]""",
            """
            Vielen Dank für Ihre Anfrage.

            Wir fertigen von Einzelprototypen bis zur Serie. Bei sehr kleinen Stückzahlen können Rüst- und Einrichtkosten den Stückpreis prägen – wir weisen dies transparent im Angebot aus.
            """.Trim()),
        new(
            "mail-response-business-hours",
            """["öffnungszeiten","geschäftszeiten","opening hours","erreichbar","telefonisch erreichbar"]""",
            """
            Vielen Dank für Ihre Nachricht.

            Unsere Geschäftszeiten sind Montag bis Freitag, 8:00–16:30 Uhr. Fachliche Anfragen zur Lohnfertigung bearbeitet unser Vertriebsteam in dieser Zeit.
            """.Trim()),
    ];
}
