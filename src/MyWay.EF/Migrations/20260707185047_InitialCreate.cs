using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyWay.EF.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "carrier_listings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    carrier_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    carrier_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    carrier_company_id = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    base_price_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    base_price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    published_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_status_changed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_visible_on_carrier_board = table.Column<bool>(type: "boolean", nullable: false, computedColumnSql: "status = 'Available'", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carrier_listings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "carrier_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    company_id = table.Column<Guid>(type: "uuid", nullable: true),
                    display_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carrier_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    legal_name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    tax_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    address_country = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    address_region = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    address_city = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    address_street = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    address_house = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    address_apartment_or_office = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    address_postal_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    address_latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    address_longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    contact_person_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    contact_phone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    contact_email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "company_members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    joined_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_members", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customer_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "driver_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    license_number = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_driver_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "resource_reservations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shipment_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: true),
                    period_start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    period_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    cancelled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource_reservations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shipment_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shipment_offers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shipment_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    carrier_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    carrier_company_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    carrier_price_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    carrier_price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    accepted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    rejected_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    withdrawn_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipment_offers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shipment_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shipment_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    accepted_offer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    customer_company_id = table.Column<Guid>(type: "uuid", nullable: true),
                    carrier_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    carrier_company_id = table.Column<Guid>(type: "uuid", nullable: true),
                    assigned_driver_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    assigned_vehicle_id = table.Column<Guid>(type: "uuid", nullable: true),
                    pickup_country = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    pickup_region = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    pickup_city = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    pickup_street = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    pickup_house = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    pickup_apartment_or_office = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    pickup_postal_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    pickup_latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    pickup_longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    delivery_country = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    delivery_region = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    delivery_city = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    delivery_street = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    delivery_house = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    delivery_apartment_or_office = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    delivery_postal_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    delivery_latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    delivery_longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    cargo_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    cargo_description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    cargo_weight_kg = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    cargo_volume_m3 = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    cargo_length_cm = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    cargo_width_cm = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    cargo_height_cm = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    cargo_is_fragile = table.Column<bool>(type: "boolean", nullable: false),
                    cargo_requires_refrigeration = table.Column<bool>(type: "boolean", nullable: false),
                    final_price_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    final_price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    platform_commission_percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    platform_commission_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    platform_commission_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    planned_pickup_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    planned_delivery_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    delivered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancelled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancellation_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipment_orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shipment_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    customer_company_id = table.Column<Guid>(type: "uuid", nullable: true),
                    target_carrier_listing_id = table.Column<Guid>(type: "uuid", nullable: true),
                    type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    pickup_country = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    pickup_region = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    pickup_city = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    pickup_street = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    pickup_house = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    pickup_apartment_or_office = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    pickup_postal_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    pickup_latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    pickup_longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    delivery_country = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    delivery_region = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    delivery_city = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    delivery_street = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    delivery_house = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    delivery_apartment_or_office = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    delivery_postal_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    delivery_latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    delivery_longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    cargo_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    cargo_description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    cargo_weight_kg = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    cargo_volume_m3 = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    cargo_length_cm = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    cargo_width_cm = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    cargo_height_cm = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    cargo_is_fragile = table.Column<bool>(type: "boolean", nullable: false),
                    cargo_requires_refrigeration = table.Column<bool>(type: "boolean", nullable: false),
                    customer_price_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    customer_price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    planned_pickup_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    planned_delivery_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    published_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancelled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancellation_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    accepted_offer_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipment_requests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    auth_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    display_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    owner_company_id = table.Column<Guid>(type: "uuid", nullable: true),
                    type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    brand = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    model = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    plate_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    capacity_kg = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    volume_m3 = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "warehouses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    owner_company_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    address_country = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    address_region = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    address_city = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    address_street = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    address_house = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    address_apartment_or_office = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    address_postal_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    address_latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    address_longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    contact_person_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    contact_phone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    contact_email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    working_hours = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    driver_comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "waybills",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shipment_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    customer_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    customer_company_id = table.Column<Guid>(type: "uuid", nullable: true),
                    carrier_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    carrier_company_id = table.Column<Guid>(type: "uuid", nullable: true),
                    driver_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    driver_license_number = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    vehicle_brand = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    vehicle_model = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    vehicle_plate_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    pickup_country = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    pickup_region = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    pickup_city = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    pickup_street = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    pickup_house = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    pickup_apartment_or_office = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    pickup_postal_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    pickup_latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    pickup_longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    delivery_country = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    delivery_region = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    delivery_city = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    delivery_street = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    delivery_house = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    delivery_apartment_or_office = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    delivery_postal_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    delivery_latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    delivery_longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    cargo_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    cargo_description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    cargo_weight_kg = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    cargo_volume_m3 = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    cargo_length_cm = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    cargo_width_cm = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    cargo_height_cm = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    cargo_is_fragile = table.Column<bool>(type: "boolean", nullable: false),
                    cargo_requires_refrigeration = table.Column<bool>(type: "boolean", nullable: false),
                    period_start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    period_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    odometer_start_km = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    odometer_end_km = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    issued_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancelled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancellation_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_waybills", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_carrier_listings_status_carrier_user_id_carrier_company_id_~",
                table: "carrier_listings",
                columns: new[] { "status", "carrier_user_id", "carrier_company_id", "is_visible_on_carrier_board" });

            migrationBuilder.CreateIndex(
                name: "IX_company_members_company_id_user_id_is_active",
                table: "company_members",
                columns: new[] { "company_id", "user_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "IX_resource_reservations_status_driver_user_id_vehicle_id_ship~",
                table: "resource_reservations",
                columns: new[] { "status", "driver_user_id", "vehicle_id", "shipment_order_id" });

            migrationBuilder.CreateIndex(
                name: "IX_reviews_shipment_order_id_target_type_target_id",
                table: "reviews",
                columns: new[] { "shipment_order_id", "target_type", "target_id" });

            migrationBuilder.CreateIndex(
                name: "IX_shipment_offers_shipment_request_id_status_carrier_user_id_~",
                table: "shipment_offers",
                columns: new[] { "shipment_request_id", "status", "carrier_user_id", "carrier_company_id" });

            migrationBuilder.CreateIndex(
                name: "IX_shipment_orders_status_customer_user_id_customer_company_id~",
                table: "shipment_orders",
                columns: new[] { "status", "customer_user_id", "customer_company_id", "carrier_user_id", "carrier_company_id" });

            migrationBuilder.CreateIndex(
                name: "IX_shipment_requests_status_type_created_at_published_at",
                table: "shipment_requests",
                columns: new[] { "status", "type", "created_at", "published_at" });

            migrationBuilder.CreateIndex(
                name: "IX_waybills_shipment_order_id_status",
                table: "waybills",
                columns: new[] { "shipment_order_id", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "carrier_listings");

            migrationBuilder.DropTable(
                name: "carrier_profiles");

            migrationBuilder.DropTable(
                name: "companies");

            migrationBuilder.DropTable(
                name: "company_members");

            migrationBuilder.DropTable(
                name: "customer_profiles");

            migrationBuilder.DropTable(
                name: "driver_profiles");

            migrationBuilder.DropTable(
                name: "resource_reservations");

            migrationBuilder.DropTable(
                name: "reviews");

            migrationBuilder.DropTable(
                name: "shipment_offers");

            migrationBuilder.DropTable(
                name: "shipment_orders");

            migrationBuilder.DropTable(
                name: "shipment_requests");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "vehicles");

            migrationBuilder.DropTable(
                name: "warehouses");

            migrationBuilder.DropTable(
                name: "waybills");
        }
    }
}
