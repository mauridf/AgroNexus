-- S009: Dados iniciais (seed)

-- Inserir usuário administrador padrão
-- Senha: Admin@123 (BCrypt hash - será substituído na aplicação)
INSERT INTO identity.users (id, email, password_hash, role) VALUES
    (uuid_generate_v4(), 'admin@agronexus.com', '$2a$11$EXEMPLO_HASH_BCRYPT_AQUI', 1)
ON CONFLICT (email) DO NOTHING;

-- Inserir culturas comuns
INSERT INTO agriculture.cultures (id, name, ciclo) VALUES
    (uuid_generate_v4(), 'Soja', 'Anual'),
    (uuid_generate_v4(), 'Milho', 'Anual'),
    (uuid_generate_v4(), 'Algodão', 'Anual'),
    (uuid_generate_v4(), 'Café', 'Perene'),
    (uuid_generate_v4(), 'Cana-de-açúcar', 'Semi-perene'),
    (uuid_generate_v4(), 'Arroz', 'Anual'),
    (uuid_generate_v4(), 'Feijão', 'Anual'),
    (uuid_generate_v4(), 'Trigo', 'Anual')
ON CONFLICT (name) DO NOTHING;

-- Inserir insumos comuns
INSERT INTO inventory.inputs (id, name, tipo, unidade_medida) VALUES
    (uuid_generate_v4(), 'Fertilizante NPK 10-10-10', 'fertilizante', 'kg'),
    (uuid_generate_v4(), 'Semente de Soja', 'semente', 'kg'),
    (uuid_generate_v4(), 'Glifosato', 'defensivo', 'L'),
    (uuid_generate_v4(), 'Óleo Diesel', 'combustível', 'L'),
    (uuid_generate_v4(), 'Calcário', 'corretivo', 'ton')
ON CONFLICT (name) DO NOTHING;