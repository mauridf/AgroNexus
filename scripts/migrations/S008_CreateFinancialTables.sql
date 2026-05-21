-- S008: Criação das tabelas do esquema Financial

-- Vendas de produção
CREATE TABLE financial.production_sales (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    planted_culture_id UUID NOT NULL,
    quantidade_vendida DECIMAL(12,4) NOT NULL,
    preco_unitario DECIMAL(15,2) NOT NULL,
    data_venda DATE NOT NULL,
    destino VARCHAR(200),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT fk_production_sales_planted_culture FOREIGN KEY (planted_culture_id) 
        REFERENCES agriculture.planted_cultures(id),
    CONSTRAINT ck_production_sales_qty CHECK (quantidade_vendida > 0),
    CONSTRAINT ck_production_sales_preco CHECK (preco_unitario > 0),
    CONSTRAINT ck_production_sales_data CHECK (data_venda <= CURRENT_DATE)
);

-- Índices
CREATE INDEX idx_production_sales_culture ON financial.production_sales(planted_culture_id);
CREATE INDEX idx_production_sales_data ON financial.production_sales(data_venda DESC);