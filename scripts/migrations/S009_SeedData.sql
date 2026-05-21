-- S009: Dados iniciais (seed)

-- Nota: O usuário administrador inicial deve ser criado via API
-- usando o endpoint POST /api/v1/auth/register
-- Isso evita problemas com hash BCrypt em scripts SQL

-- Inserir culturas comuns
INSERT INTO agriculture.cultures (id, name, ciclo) 
SELECT gen_random_uuid(), 'Soja', 'Anual'
WHERE NOT EXISTS (SELECT 1 FROM agriculture.cultures WHERE name = 'Soja');

INSERT INTO agriculture.cultures (id, name, ciclo) 
SELECT gen_random_uuid(), 'Milho', 'Anual'
WHERE NOT EXISTS (SELECT 1 FROM agriculture.cultures WHERE name = 'Milho');

INSERT INTO agriculture.cultures (id, name, ciclo) 
SELECT gen_random_uuid(), 'Algodão', 'Anual'
WHERE NOT EXISTS (SELECT 1 FROM agriculture.cultures WHERE name = 'Algodão');

INSERT INTO agriculture.cultures (id, name, ciclo) 
SELECT gen_random_uuid(), 'Café', 'Perene'
WHERE NOT EXISTS (SELECT 1 FROM agriculture.cultures WHERE name = 'Café');

INSERT INTO agriculture.cultures (id, name, ciclo) 
SELECT gen_random_uuid(), 'Cana-de-açúcar', 'Semi-perene'
WHERE NOT EXISTS (SELECT 1 FROM agriculture.cultures WHERE name = 'Cana-de-açúcar');

INSERT INTO agriculture.cultures (id, name, ciclo) 
SELECT gen_random_uuid(), 'Arroz', 'Anual'
WHERE NOT EXISTS (SELECT 1 FROM agriculture.cultures WHERE name = 'Arroz');

INSERT INTO agriculture.cultures (id, name, ciclo) 
SELECT gen_random_uuid(), 'Feijão', 'Anual'
WHERE NOT EXISTS (SELECT 1 FROM agriculture.cultures WHERE name = 'Feijão');

INSERT INTO agriculture.cultures (id, name, ciclo) 
SELECT gen_random_uuid(), 'Trigo', 'Anual'
WHERE NOT EXISTS (SELECT 1 FROM agriculture.cultures WHERE name = 'Trigo');

-- Inserir insumos comuns
INSERT INTO inventory.inputs (id, name, tipo, unidade_medida) 
SELECT gen_random_uuid(), 'Fertilizante NPK 10-10-10', 'fertilizante', 'kg'
WHERE NOT EXISTS (SELECT 1 FROM inventory.inputs WHERE name = 'Fertilizante NPK 10-10-10');

INSERT INTO inventory.inputs (id, name, tipo, unidade_medida) 
SELECT gen_random_uuid(), 'Semente de Soja', 'semente', 'kg'
WHERE NOT EXISTS (SELECT 1 FROM inventory.inputs WHERE name = 'Semente de Soja');

INSERT INTO inventory.inputs (id, name, tipo, unidade_medida) 
SELECT gen_random_uuid(), 'Glifosato', 'defensivo', 'L'
WHERE NOT EXISTS (SELECT 1 FROM inventory.inputs WHERE name = 'Glifosato');

INSERT INTO inventory.inputs (id, name, tipo, unidade_medida) 
SELECT gen_random_uuid(), 'Óleo Diesel', 'combustível', 'L'
WHERE NOT EXISTS (SELECT 1 FROM inventory.inputs WHERE name = 'Óleo Diesel');

INSERT INTO inventory.inputs (id, name, tipo, unidade_medida) 
SELECT gen_random_uuid(), 'Calcário', 'corretivo', 'ton'
WHERE NOT EXISTS (SELECT 1 FROM inventory.inputs WHERE name = 'Calcário');