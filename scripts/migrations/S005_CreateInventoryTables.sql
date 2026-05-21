-- S005: Criação das tabelas do esquema Inventory

-- Catálogo de insumos
CREATE TABLE inventory.inputs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(150) NOT NULL,
    tipo VARCHAR(50), -- fertilizante, semente, defensivo, combustível
    unidade_medida VARCHAR(20), -- kg, L, ton, saca, unidade
    fornecedor VARCHAR(200),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT uq_inputs_name UNIQUE (name)
);

-- Compras de insumos
CREATE TABLE inventory.input_purchases (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    farm_id UUID NOT NULL,
    input_id UUID NOT NULL,
    quantidade DECIMAL(12,4) NOT NULL,
    valor_total DECIMAL(15,2) NOT NULL,
    data_compra DATE NOT NULL,
    fornecedor VARCHAR(200),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT fk_input_purchases_farm FOREIGN KEY (farm_id) REFERENCES farm.farms(id),
    CONSTRAINT fk_input_purchases_input FOREIGN KEY (input_id) REFERENCES inventory.inputs(id),
    CONSTRAINT ck_input_purchases_qty CHECK (quantidade > 0),
    CONSTRAINT ck_input_purchases_valor CHECK (valor_total > 0),
    CONSTRAINT ck_input_purchases_data CHECK (data_compra <= CURRENT_DATE)
);

-- Estoque de insumos por fazenda
CREATE TABLE inventory.input_stocks (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    farm_id UUID NOT NULL,
    input_id UUID NOT NULL,
    quantidade DECIMAL(12,4) NOT NULL DEFAULT 0,
    validade DATE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT fk_input_stocks_farm FOREIGN KEY (farm_id) REFERENCES farm.farms(id),
    CONSTRAINT fk_input_stocks_input FOREIGN KEY (input_id) REFERENCES inventory.inputs(id),
    CONSTRAINT ck_input_stocks_qty CHECK (quantidade >= 0),
    CONSTRAINT uq_input_stocks_farm_input UNIQUE (farm_id, input_id)
);

-- Índices
CREATE INDEX idx_input_purchases_farm_id ON inventory.input_purchases(farm_id);
CREATE INDEX idx_input_purchases_data ON inventory.input_purchases(data_compra DESC);
CREATE INDEX idx_input_stocks_farm_id ON inventory.input_stocks(farm_id) WHERE is_active = TRUE;