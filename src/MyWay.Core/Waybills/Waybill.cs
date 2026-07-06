using MyWay.Core.Common.Exceptions;
using MyWay.Core.Common.ValueObjects;

namespace MyWay.Core.Waybills;

public sealed class Waybill
{
    private Waybill(
        Guid id,
        Guid shipmentOrderId,
        string number,
        Guid? customerUserId,
        Guid? customerCompanyId,
        Guid? carrierUserId,
        Guid? carrierCompanyId,
        Guid driverUserId,
        Guid vehicleId,
        string? driverName,
        string? driverLicenseNumber,
        string? vehicleBrand,
        string? vehicleModel,
        string vehiclePlateNumber,
        Address pickupAddress,
        Address deliveryAddress,
        CargoDetails cargoDetails,
        DateRange tripPeriod,
        decimal? odometerStartKm,
        string? comment,
        DateTimeOffset createdAt)
    {
        Id = id;
        ShipmentOrderId = shipmentOrderId;
        Number = number;
        Status = WaybillStatus.Draft;
        CustomerUserId = customerUserId;
        CustomerCompanyId = customerCompanyId;
        CarrierUserId = carrierUserId;
        CarrierCompanyId = carrierCompanyId;
        DriverUserId = driverUserId;
        VehicleId = vehicleId;
        DriverName = driverName;
        DriverLicenseNumber = driverLicenseNumber;
        VehicleBrand = vehicleBrand;
        VehicleModel = vehicleModel;
        VehiclePlateNumber = vehiclePlateNumber;
        PickupAddress = pickupAddress;
        DeliveryAddress = deliveryAddress;
        CargoDetails = cargoDetails;
        TripPeriod = tripPeriod;
        OdometerStartKm = odometerStartKm;
        Comment = comment;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid ShipmentOrderId { get; private set; }

    public string Number { get; private set; }

    public WaybillStatus Status { get; private set; }

    public Guid? CustomerUserId { get; private set; }

    public Guid? CustomerCompanyId { get; private set; }

    public Guid? CarrierUserId { get; private set; }

    public Guid? CarrierCompanyId { get; private set; }

    public Guid DriverUserId { get; private set; }

    public Guid VehicleId { get; private set; }

    public string? DriverName { get; private set; }

    public string? DriverLicenseNumber { get; private set; }

    public string? VehicleBrand { get; private set; }

    public string? VehicleModel { get; private set; }

    public string VehiclePlateNumber { get; private set; }

    public Address PickupAddress { get; private set; }

    public Address DeliveryAddress { get; private set; }

    public CargoDetails CargoDetails { get; private set; }

    public DateRange TripPeriod { get; private set; }

    public decimal? OdometerStartKm { get; private set; }

    public decimal? OdometerEndKm { get; private set; }

    public string? Comment { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? IssuedAt { get; private set; }

    public DateTimeOffset? ClosedAt { get; private set; }

    public DateTimeOffset? CancelledAt { get; private set; }

    public string? CancellationReason { get; private set; }

    public static Waybill Create(
        Guid shipmentOrderId,
        string number,
        Guid? customerUserId,
        Guid? customerCompanyId,
        Guid? carrierUserId,
        Guid? carrierCompanyId,
        Guid driverUserId,
        Guid vehicleId,
        string vehiclePlateNumber,
        Address pickupAddress,
        Address deliveryAddress,
        CargoDetails cargoDetails,
        DateRange tripPeriod,
        DateTimeOffset createdAt,
        string? driverName = null,
        string? driverLicenseNumber = null,
        string? vehicleBrand = null,
        string? vehicleModel = null,
        decimal? odometerStartKm = null,
        string? comment = null)
    {
        ValidateShipmentOrderId(shipmentOrderId);
        ValidateNumber(number);
        var normalizedCustomerUserId = NormalizeOptionalId(customerUserId);
        var normalizedCustomerCompanyId = NormalizeOptionalId(customerCompanyId);
        var normalizedCarrierUserId = NormalizeOptionalId(carrierUserId);
        var normalizedCarrierCompanyId = NormalizeOptionalId(carrierCompanyId);
        ValidateParty(normalizedCustomerUserId, normalizedCustomerCompanyId, "customer");
        ValidateParty(normalizedCarrierUserId, normalizedCarrierCompanyId, "carrier");
        ValidateDriverUserId(driverUserId);
        ValidateVehicleId(vehicleId);
        ValidateVehiclePlateNumber(vehiclePlateNumber);
        ValidateRequiredObjects(pickupAddress, deliveryAddress, cargoDetails, tripPeriod);
        ValidateOdometerValues(odometerStartKm, null);

        return new Waybill(
            Guid.NewGuid(),
            shipmentOrderId,
            number.Trim(),
            normalizedCustomerUserId,
            normalizedCustomerCompanyId,
            normalizedCarrierUserId,
            normalizedCarrierCompanyId,
            driverUserId,
            vehicleId,
            NormalizeOptional(driverName),
            NormalizeOptional(driverLicenseNumber),
            NormalizeOptional(vehicleBrand),
            NormalizeOptional(vehicleModel),
            vehiclePlateNumber.Trim(),
            pickupAddress,
            deliveryAddress,
            cargoDetails,
            tripPeriod,
            odometerStartKm,
            NormalizeOptional(comment),
            createdAt);
    }

    public void UpdateInfo(
        string number,
        string vehiclePlateNumber,
        Address pickupAddress,
        Address deliveryAddress,
        CargoDetails cargoDetails,
        DateRange tripPeriod,
        string? driverName = null,
        string? driverLicenseNumber = null,
        string? vehicleBrand = null,
        string? vehicleModel = null,
        decimal? odometerStartKm = null,
        string? comment = null)
    {
        if (Status != WaybillStatus.Draft)
        {
            throw new DomainException("Only draft waybills can be updated.");
        }

        ValidateNumber(number);
        ValidateVehiclePlateNumber(vehiclePlateNumber);
        ValidateRequiredObjects(pickupAddress, deliveryAddress, cargoDetails, tripPeriod);
        ValidateOdometerValues(odometerStartKm, OdometerEndKm);

        Number = number.Trim();
        VehiclePlateNumber = vehiclePlateNumber.Trim();
        PickupAddress = pickupAddress;
        DeliveryAddress = deliveryAddress;
        CargoDetails = cargoDetails;
        TripPeriod = tripPeriod;
        DriverName = NormalizeOptional(driverName);
        DriverLicenseNumber = NormalizeOptional(driverLicenseNumber);
        VehicleBrand = NormalizeOptional(vehicleBrand);
        VehicleModel = NormalizeOptional(vehicleModel);
        OdometerStartKm = odometerStartKm;
        Comment = NormalizeOptional(comment);
    }

    public void Issue(DateTimeOffset issuedAt)
    {
        if (Status != WaybillStatus.Draft)
        {
            throw new DomainException("Only draft waybills can be issued.");
        }

        Status = WaybillStatus.Issued;
        IssuedAt = issuedAt;
    }

    public void Close(DateTimeOffset closedAt, decimal? odometerEndKm = null)
    {
        if (Status != WaybillStatus.Issued)
        {
            throw new DomainException("Only issued waybills can be closed.");
        }

        ValidateOdometerValues(OdometerStartKm, odometerEndKm);

        Status = WaybillStatus.Closed;
        OdometerEndKm = odometerEndKm;
        ClosedAt = closedAt;
    }

    public void Cancel(string? reason, DateTimeOffset cancelledAt)
    {
        if (Status is not WaybillStatus.Draft and not WaybillStatus.Issued)
        {
            throw new DomainException("Only draft or issued waybills can be cancelled.");
        }

        Status = WaybillStatus.Cancelled;
        CancellationReason = NormalizeOptional(reason);
        CancelledAt = cancelledAt;
    }

    private static void ValidateShipmentOrderId(Guid shipmentOrderId)
    {
        if (shipmentOrderId == Guid.Empty)
        {
            throw new DomainException("Shipment order id is required.");
        }
    }

    private static void ValidateNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            throw new DomainException("Waybill number is required.");
        }
    }

    private static void ValidateParty(Guid? userId, Guid? companyId, string partyName)
    {
        var hasUser = userId.HasValue;
        var hasCompany = companyId.HasValue;

        if (!hasUser && !hasCompany)
        {
            throw new DomainException($"Waybill {partyName} is required.");
        }

        if (hasUser && hasCompany)
        {
            throw new DomainException($"Waybill cannot have both user and company {partyName}.");
        }
    }

    private static void ValidateDriverUserId(Guid driverUserId)
    {
        if (driverUserId == Guid.Empty)
        {
            throw new DomainException("Driver user id is required.");
        }
    }

    private static void ValidateVehicleId(Guid vehicleId)
    {
        if (vehicleId == Guid.Empty)
        {
            throw new DomainException("Vehicle id is required.");
        }
    }

    private static void ValidateVehiclePlateNumber(string vehiclePlateNumber)
    {
        if (string.IsNullOrWhiteSpace(vehiclePlateNumber))
        {
            throw new DomainException("Vehicle plate number is required.");
        }
    }

    private static void ValidateRequiredObjects(
        Address? pickupAddress,
        Address? deliveryAddress,
        CargoDetails? cargoDetails,
        DateRange? tripPeriod)
    {
        if (pickupAddress is null)
        {
            throw new DomainException("Pickup address is required.");
        }

        if (deliveryAddress is null)
        {
            throw new DomainException("Delivery address is required.");
        }

        if (cargoDetails is null)
        {
            throw new DomainException("Cargo details are required.");
        }

        if (tripPeriod is null)
        {
            throw new DomainException("Trip period is required.");
        }
    }

    private static void ValidateOdometerValues(decimal? odometerStartKm, decimal? odometerEndKm)
    {
        if (odometerStartKm.HasValue && odometerStartKm.Value < 0)
        {
            throw new DomainException("Odometer start cannot be negative.");
        }

        if (odometerEndKm.HasValue && odometerEndKm.Value < 0)
        {
            throw new DomainException("Odometer end cannot be negative.");
        }

        if (odometerStartKm.HasValue
            && odometerEndKm.HasValue
            && odometerEndKm.Value < odometerStartKm.Value)
        {
            throw new DomainException("Odometer end cannot be less than odometer start.");
        }
    }

    private static Guid? NormalizeOptionalId(Guid? id)
    {
        return id.HasValue && id.Value != Guid.Empty ? id.Value : null;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
