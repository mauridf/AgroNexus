-- S004: Criação das tabelas do esquema Agriculture

-- Catálogo de culturas
CREATE TABLE agriculture.cultures (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(100) NOT NULL,
    ciclo VARCHAR(50), -- Anual, Perene, Semi-perene
    variedade VARCHAR(100),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT uq_cultures_name UNIQUE (name)
);

-- Culturas plantadas nas fazendas
CREATE TABLE agriculture.planted_cultures (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    farm_id UUID NOT NULL,
    culture_id UUID NOT NULL,
    safra VARCHAR(50) NOT NULL,
    area_plantada_ha DECIMAL(12,4) NOT NULL,
    data_plantio DATE,
    data_colheita_prevista DATE,
    data_colheita_real DATE,
    produtividade_esperada_sacas_ha DECIMAL(12,4),
    produtividade_obtida_sacas_ha DECIMAL(12,4),
    custo_total DECIMAL(15,2),
    receita_total DECIMAL(15,2),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT fk_planted_cultures_farm FOREIGN KEY (farm_id) REFERENCES farm.farms(id),
    CONSTRAINT fk_planted_cultures_culture FOREIGN KEY (culture_id) REFERENCES agriculture.cultures(id),
    CONSTRAINT ck_planted_cultures_area CHECK (area_plantada_ha > 0),
    CONSTRAINT ck_planted_cultures_harvest_date CHECK (
        data_colheita_real IS NULL OR data_plantio IS NULL OR data_colheita_real >= data_plantio
    ),
    CONSTRAINT ck_planted_cultures_produtividade CHECK (
        produtividade_obtida_sacas_ha IS NULL OR produtividade_obtida_sacas_ha >= 0
    )
);

-- Índices
CREATE INDEX idx_planted_cultures_farm_id ON agriculture.planted_cultures(farm_id) WHERE is_active = TRUE;
CREATE INDEX idx_planted_cultures_safra ON agriculture.planted_cultures(farm_id, safra);
CREATE INDEX idx_planted_cultures_culture_id ON agriculture.planted_cultures(culture_id);