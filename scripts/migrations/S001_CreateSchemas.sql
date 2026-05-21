-- S001: Criação dos esquemas do banco de dados
-- Esquemas organizam as tabelas por módulo, melhorando segurança e manutenibilidade

-- Extensão para UUID (já vem habilitada no PostgreSQL 13+, mas garantimos)
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE SCHEMA IF NOT EXISTS identity;
CREATE SCHEMA IF NOT EXISTS farm;
CREATE SCHEMA IF NOT EXISTS agriculture;
CREATE SCHEMA IF NOT EXISTS inventory;
CREATE SCHEMA IF NOT EXISTS operations;
CREATE SCHEMA IF NOT EXISTS monitoring;
CREATE SCHEMA IF NOT EXISTS financial;

COMMENT ON SCHEMA identity IS 'Autenticação, autorização e usuários';
COMMENT ON SCHEMA farm IS 'Produtores, fazendas e funcionários';
COMMENT ON SCHEMA agriculture IS 'Culturas e culturas plantadas';
COMMENT ON SCHEMA inventory IS 'Insumos, compras e estoque';
COMMENT ON SCHEMA operations IS 'Contratos, custos operacionais e máquinas';
COMMENT ON SCHEMA monitoring IS 'Alertas, certificados e clima';
COMMENT ON SCHEMA financial IS 'Vendas de produção e finanças';