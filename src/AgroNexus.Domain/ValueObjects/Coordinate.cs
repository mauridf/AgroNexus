using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.ValueObjects;

/// <summary>
/// Value Object que representa coordenadas geográficas (latitude e longitude).
/// </summary>
public sealed record Coordinate
{
    /// <summary>
    /// Latitude (-90 a +90 graus).
    /// </summary>
    public decimal Latitude { get; }

    /// <summary>
    /// Longitude (-180 a +180 graus).
    /// </summary>
    public decimal Longitude { get; }

    private Coordinate(decimal latitude, decimal longitude)
    {
        Latitude = Math.Round(latitude, 6);
        Longitude = Math.Round(longitude, 6);
    }

    public static Coordinate Create(decimal latitude, decimal longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new DomainException("Latitude deve estar entre -90 e +90.", "COORD_LAT_INVALID");

        if (longitude < -180 || longitude > 180)
            throw new DomainException("Longitude deve estar entre -180 e +180.", "COORD_LNG_INVALID");

        return new Coordinate(latitude, longitude);
    }

    public override string ToString() => $"({Latitude}, {Longitude})";
}