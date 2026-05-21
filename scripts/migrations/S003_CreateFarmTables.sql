-- S003: Criação das tabelas do esquema Farm

-- Tabela de produtores
CREATE TABLE farm.producers (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL,
    name VARCHAR(200) NOT NULL,
    cpf_cnpj VARCHAR(14) NOT NULL, -- Apenas números: 11 (CPF) ou 14 (CNPJ)
    rg VARCHAR(20),
    inscricao_estadual VARCHAR(20),
    data_nascimento DATE,
    telefone VARCHAR(20),
    endereco VARCHAR(255),
    cidade VARCHAR(100),
    estado CHAR(2),
    dados_bancarios VARCHAR(255),
    car VARCHAR(50),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT fk_producers_user FOREIGN KEY (user_id) REFERENCES identity.users(id),
    CONSTRAINT uq_producers_cpf_cnpj UNIQUE (cpf_cnpj),
    CONSTRAINT uq_producers_user UNIQUE (user_id), -- Um usuário = um produtor
    CONSTRAINT ck_producers_cpf_cnpj_length CHECK (LENGTH(cpf_cnpj) IN (11, 14)),
    CONSTRAINT ck_producers_estado CHECK (estado ~ '^[A-Z]{2}$')
);

-- Tabela de fazendas
CREATE TABLE farm.farms (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    producer_id UUID NOT NULL,
    name VARCHAR(200) NOT NULL,
    endereco VARCHAR(255),
    cidade VARCHAR(100),
    estado CHAR(2),
    latitude DECIMAL(9,6),
    longitude DECIMAL(9,6),
    total_area_ha DECIMAL(12,4) NOT NULL,
    agricultural_area_ha DECIMAL(12,4) NOT NULL DEFAULT 0,
    vegetation_area_ha DECIMAL(12,4) NOT NULL DEFAULT 0,
    built_area_ha DECIMAL(12,4) NOT NULL DEFAULT 0,
    inscricao_estadual VARCHAR(20),
    codigo_car VARCHAR(50),
    ccir VARCHAR(50),
    fonte_agua VARCHAR(100),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT fk_farms_producer FOREIGN KEY (producer_id) REFERENCES farm.producers(id),
    CONSTRAINT ck_farms_area_total CHECK (total_area_ha > 0),
    CONSTRAINT ck_farms_areas_sum CHECK (
        agricultural_area_ha + vegetation_area_ha + built_area_ha <= total_area_ha
    ),
    CONSTRAINT ck_farms_areas_positive CHECK (
        agricultural_area_ha >= 0 AND vegetation_area_ha >= 0 AND built_area_ha >= 0
    ),
    CONSTRAINT ck_farms_latitude CHECK (latitude >= -90 AND latitude <= 90),
    CONSTRAINT ck_farms_longitude CHECK (longitude >= -180 AND longitude <= 180),
    CONSTRAINT ck_farms_estado CHECK (estado IS NULL OR estado ~ '^[A-Z]{2}$')
);

-- Tabela de funcionários
CREATE TABLE farm.employees (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    farm_id UUID NOT NULL,
    name VARCHAR(200) NOT NULL,
    cpf VARCHAR(11),
    funcao VARCHAR(100),
    salario DECIMAL(12,2),
    data_admissao DATE,
    data_demissao DATE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT fk_employees_farm FOREIGN KEY (farm_id) REFERENCES farm.farms(id),
    CONSTRAINT ck_employees_cpf_length CHECK (cpf IS NULL OR LENGTH(cpf) = 11),
    CONSTRAINT ck_employees_salario CHECK (salario IS NULL OR salario > 0),
    CONSTRAINT ck_employees_dates CHECK (
        data_demissao IS NULL OR data_admissao IS NULL OR data_demissao >= data_admissao
    )
);

-- Índices
CREATE INDEX idx_producers_cpf_cnpj ON farm.producers(cpf_cnpj) WHERE is_active = TRUE;
CREATE INDEX idx_producers_user_id ON farm.producers(user_id);
CREATE INDEX idx_farms_producer_id ON farm.farms(producer_id) WHERE is_active = TRUE;
CREATE INDEX idx_farms_cidade ON farm.farms(cidade) WHERE is_active = TRUE;
CREATE INDEX idx_employees_farm_id ON farm.employees(farm_id) WHERE is_active = TRUE;