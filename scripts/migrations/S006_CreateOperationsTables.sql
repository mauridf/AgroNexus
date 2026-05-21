-- S006: Criação das tabelas do esquema Operations

-- Contratos
CREATE TABLE operations.contracts (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    farm_id UUID NOT NULL,
    tipo VARCHAR(50) NOT NULL, -- Arrendamento, Parceria, Financiamento
    parte_contratante VARCHAR(200),
    valor DECIMAL(15,2) NOT NULL,
    data_inicio DATE NOT NULL,
    data_fim DATE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT fk_contracts_farm FOREIGN KEY (farm_id) REFERENCES farm.farms(id),
    CONSTRAINT ck_contracts_valor CHECK (valor > 0),
    CONSTRAINT ck_contracts_dates CHECK (data_fim IS NULL OR data_fim >= data_inicio)
);

-- Custos operacionais
CREATE TABLE operations.operational_costs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    farm_id UUID NOT NULL,
    descricao VARCHAR(255) NOT NULL,
    valor DECIMAL(15,2) NOT NULL,
    data DATE NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT fk_operational_costs_farm FOREIGN KEY (farm_id) REFERENCES farm.farms(id),
    CONSTRAINT ck_operational_costs_valor CHECK (valor > 0),
    CONSTRAINT ck_operational_costs_data CHECK (data <= CURRENT_DATE)
);

-- Máquinas agrícolas
CREATE TABLE operations.machines (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    farm_id UUID NOT NULL,
    descricao VARCHAR(255) NOT NULL,
    marca VARCHAR(100),
    modelo VARCHAR(100),
    ano INT,
    valor_aproximado DECIMAL(15,2),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT fk_machines_farm FOREIGN KEY (farm_id) REFERENCES farm.farms(id),
    CONSTRAINT ck_machines_ano CHECK (ano IS NULL OR (ano >= 1900 AND ano <= EXTRACT(YEAR FROM CURRENT_DATE) + 1))
);

-- Índices
CREATE INDEX idx_contracts_farm_id ON operations.contracts(farm_id) WHERE is_active = TRUE;
CREATE INDEX idx_operational_costs_farm_id ON operations.operational_costs(farm_id) WHERE is_active = TRUE;
CREATE INDEX idx_operational_costs_data ON operations.operational_costs(farm_id, data DESC);
CREATE INDEX idx_machines_farm_id ON operations.machines(farm_id) WHERE is_active = TRUE;