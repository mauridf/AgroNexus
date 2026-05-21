-- S002: Criação das tabelas do esquema Identity

-- Tabela de usuários
CREATE TABLE identity.users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    email VARCHAR(255) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role SMALLINT NOT NULL CHECK (role IN (1, 2)), -- 1=ADM, 2=PRD
    last_login TIMESTAMPTZ,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    -- Constraints
    CONSTRAINT uq_users_email UNIQUE (email)
);

-- Índices
CREATE INDEX idx_users_email ON identity.users(email) WHERE is_active = TRUE;
CREATE INDEX idx_users_role ON identity.users(role) WHERE is_active = TRUE;

-- Comentários
COMMENT ON TABLE identity.users IS 'Usuários do sistema (Administradores e Produtores)';
COMMENT ON COLUMN identity.users.role IS '1=ADM (Administrador), 2=PRD (Produtor)';