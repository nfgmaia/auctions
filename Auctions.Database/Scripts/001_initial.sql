CREATE TYPE vehicle_type AS ENUM ('hatchback', 'sedan', 'suv', 'truck');

CREATE TABLE IF NOT EXISTS vehicle
(
    id              TEXT            PRIMARY KEY,
    manufacturer    TEXT            NOT NULL,
    model           TEXT            NOT NULL,
    "type"          vehicle_type    NOT NULL,
    "year"          INT             NOT NULL,
    "starting_bid"  BIGINT          NOT NULL,
    "nr_doors"      INT             NULL,
    "nr_seats"      INT             NULL,
    "load_capacity" INT             NULL,
    created_at      TIMESTAMPTZ     NOT NULL        DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS auction
(
    id              TEXT            PRIMARY KEY,
    vehicle_id      TEXT            NOT NULL,
    start_date      TIMESTAMPTZ     NOT NULL,
    end_date        TIMESTAMPTZ     NULL,
    starting_bid    BIGINT          NOT NULL,
    current_bid     BIGINT          NULL,
    last_bid_at     TIMESTAMPTZ     NULL,
    last_bidder     TEXT            NULL,
    created_at      TIMESTAMPTZ     NOT NULL        DEFAULT NOW(),  
    
    CONSTRAINT fk_vehicle FOREIGN KEY (vehicle_id) REFERENCES vehicle (id)
);

CREATE INDEX IF NOT EXISTS idx_auction_vehicle_id ON auction (vehicle_id);

