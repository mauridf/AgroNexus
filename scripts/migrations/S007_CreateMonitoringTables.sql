-- S007: Criação das tabelas do esquema Monitoring

-- Alertas de monitoramento
CREATE TABLE monitoring.alerts (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    farm_id UUID NOT NULL,
    tipo VARCHAR(50) NOT NULL, -- praga, doença, clima, maquinário
    descricao TEXT,
    nivel VARCHAR(10) NOT NULL CHECK (nivel IN ('baixo', 'medio', 'alto')),
    data DATE NOT NULL,
    resolvido BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT fk_alerts_farm FOREIGN KEY (farm_id) REFERENCES farm.farms(id)
);

-- Certificados
CREATE TABLE monitoring.certificates (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    farm_id UUID NOT NULL,
    tipo VARCHAR(100) NOT NULL, -- Orgânico, FairTrade, Rainforest Alliance
    data_emissao DATE NOT NULL,
    data_validade DATE NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT fk_certificates_farm FOREIGN KEY (farm_id) REFERENCES farm.farms(id),
    CONSTRAINT ck_certificates_dates CHECK (data_validade > data_emissao)
);

-- Registros climáticos
CREATE TABLE monitoring.climates (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    farm_id UUID NOT NULL,
    data DATE NOT NULL,
    temperatura DECIMAL(5,2),
    chuva_mm DECIMAL(8,2),
    umidade DECIMAL(5,2),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT fk_climates_farm FOREIGN KEY (farm_id) REFERENCES farm.farms(id),
    CONSTRAINT ck_climates_umidade CHECK (umidade IS NULL OR (umidade >= 0 AND umidade <= 100)),
    CONSTRAINT ck_climates_chuva CHECK (chuva_mm IS NULL OR chuva_mm >= 0),
    CONSTRAINT uq_climates_farm_data UNIQUE (farm_id, data)
);

-- Índices
CREATE INDEX idx_alerts_farm_id ON monitoring.alerts(farm_id) WHERE is_active = TRUE;
CREATE INDEX idx_alerts_resolvido ON monitoring.alerts(farm_id, resolvido);
CREATE INDEX idx_certificates_farm_id ON monitoring.certificates(farm_id) WHERE is_active = TRUE;
CREATE INDEX idx_certificates_validade ON monitoring.certificates(data_validade DESC);
CREATE INDEX idx_climates_farm_id ON monitoring.climates(farm_id) WHERE is_active = TRUE;
CREATE INDEX idx_climates_data ON monitoring.climates(farm_id, data DESC);