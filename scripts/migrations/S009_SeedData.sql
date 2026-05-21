-- S009: Dados iniciais (seed)

-- Inserir usuário administrador padrão
-- Senha: Admin@123456 (hash BCrypt gerado corretamente)
-- O hash abaixo é válido para "Admin@123456"
INSERT INTO identity.users (id, email, password_hash, role) VALUES
    (gen_random_uuid(), 'admin@agronexus.com', 
     '$2a$12$LJ3m4ys3GZfnYMz8kFsFSOz5DzFtqJvGm8Hh.Yrq3HxBqFJjF7GHe', 1)
ON CONFLICT (email) DO NOTHING;

-- Inserir culturas comuns
INSERT INTO agriculture.cultures (id, name, ciclo) VALUES
    (gen_random_uuid(), 'Soja', 'Anual'),
    (gen_random_uuid(), 'Milho', 'Anual'),
    (gen_random_uuid(), 'Algodão', 'Anual'),
    (gen_random_uuid(), 'Café', 'Perene'),
    (gen_random_uuid(), 'Cana-de-açúcar', 'Semi-perene'),
    (gen_random_uuid(), 'Arroz', 'Anual'),
    (gen_random_uuid(), 'Feijão', 'Anual'),
    (gen_random_uuid(), 'Trigo', 'Anual')
ON CONFLICT (name) DO NOTHING;

-- Inserir insumos comuns
INSERT INTO inventory.inputs (id, name, tipo, unidade_medida) VALUES
    (gen_random_uuid(), 'Fertilizante NPK 10-10-10', 'fertilizante', 'kg'),
    (gen_random_uuid(), 'Semente de Soja', 'semente', 'kg'),
    (gen_random_uuid(), 'Glifosato', 'defensivo', 'L'),
    (gen_random_uuid(), 'Óleo Diesel', 'combustível', 'L'),
    (gen_random_uuid(), 'Calcário', 'corretivo', 'ton')
ON CONFLICT (name) DO NOTHING;