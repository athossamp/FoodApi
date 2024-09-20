drop table if exists comanda_item_log;
drop table if exists comanda_item_preparo;
drop table if exists tipo_log_item;
drop table if exists comanda_item_composicao;
drop table if exists comanda_item;
drop table if exists comanda;
drop table if exists local_atendimento;
drop table if exists produto_composicao_substituto;
drop table if exists produto_composicao;
drop table if exists produto;
drop table if exists classmerc;
drop table if exists cardapio;
drop table if exists vendedor;
drop table if exists empresa;
drop table if exists usuario;


drop sequence if exists cmilcodigo;
drop sequence if exists cmipcodigo;
drop sequence if exists tlicodigo;
drop sequence if exists cmdicodigo;
drop sequence if exists cmdcodigo;
drop sequence if exists pcsbcodigo;
drop sequence if exists pcpcodigo;
drop sequence if exists procodigo;
drop sequence if exists cdpcodigo;
drop sequence if exists latcodigo;
drop sequence if exists clmcodigo;
drop sequence if exists usucodigo;
drop sequence if exists vencodigo;
drop sequence if exists empcodigo;
drop sequence if exists usucodigo;
drop sequence if exists ciccodigo;

/*create sequence if not exists cdpcodigo;
create sequence if not exists clmcodigo; 
create sequence if not exists cmdcodigo;
create sequence if not exists cmdicodigo;
create sequence if not exists latcodigo;
create sequence if not exists pcpcodigo;
create sequence if not exists pcsbcodigo;
create sequence if not exists procodigo;
create sequence if not exists tlicodigo;
create sequence if not exists usucodigo;
create sequence if not exists empcodigo;
*/

create sequence if not exists usucodigo;
create table usuario (
    usucodigo                smallint not null,
    usunome                  varchar(60) not null,
    usuapelido		     	 varchar(60) not null,
    usumeta                  numeric(15,2) default 0,
    usunivel                 smallint default 0 not null,
    usustatus                char(1) default 'A',
    usutipo                  char(1) default 'N',
    usuemail                 varchar(120),
	usutelefone_ddd          varchar(2),
    usutelefone              varchar(40),
    usucpf 					 varchar(11),
    usucomissao              numeric(15,4) default 0.00,
    usulogin				 varchar(20) not null,
    ususenha                 varchar(255) not null,
    usunumero_origem 		 int,
    usutoken				 text,
    usutoken_expira			 timestamp,
    transa int,
    usucodigo_insercao smallint not null,
    ultatualizacao timestamp default now(),
    constraint pk_usu primary key (usucodigo),
    constraint uk_usu unique(usucpf)
);
INSERT INTO usuario (usucodigo, usunome, usuapelido, usumeta, usunivel, usustatus, usutipo, usuemail, usutelefone_ddd, usutelefone, usucpf, usucomissao, usulogin, ususenha, usunumero_origem, usutoken, usutoken_expira, transa, usucodigo_insercao, ultatualizacao) 
  VALUES(1, 'Logicom', 'Logicom', 0, 0, 'A', 'V', null, '91', '32229756', '00000000000', 0.00, 'logicom', '123', 0, null, null, 0, 0, now());
 
 INSERT INTO usuario (usucodigo, usunome, usuapelido, usumeta, usunivel, usustatus, usutipo, usuemail, usutelefone_ddd, usutelefone, usucpf, usucomissao, usulogin, ususenha, usunumero_origem, usutoken, usutoken_expira, transa, usucodigo_insercao, ultatualizacao) 
  VALUES(2, 'Vendedor genérico', 'Vendedor genérico', 0, 0, 'A', 'V', null, '91', '32229756', '11111111111', 0.00, 'vendedor', '123', 0, null, null, 0, 0, now()); 

INSERT INTO usuario (usucodigo, usunome, usuapelido, usumeta, usunivel, usustatus, usutipo, usuemail, usutelefone_ddd, usutelefone, usucpf, usucomissao, usulogin, ususenha, usunumero_origem, usutoken, usutoken_expira, transa, usucodigo_insercao, ultatualizacao) 
 VALUES(3, 'Vendedor e Gerente', 'Vendedor e Gerente', 0, 9, 'A', 'G', null, '91', '32229756', '22222222222', 0.00, 'gerente', '123456', 0, null, null, 0, 0, now()); 
 

create sequence if not exists empcodigo; 
create table empresa (
    empcodigo                  smallint not null,
    empnome                    varchar(60) not null,
    empfantasia                varchar(60) not null,
    empendereco                varchar(60) not null,
    empbairro                  varchar(60) not null,
    empcidade                  varchar(60) not null,
    ufcodigo                   varchar(2) not null,
    empfone_ddd                varchar(2),
    empfone                    varchar(11),
    empe_mail                  varchar(60),
    empcep                     varchar(12),
    empie                      varchar(18),
    empcnpj                    varchar(20) not null,
    empcertificadodigital      varchar(512),
    empcertificadodigitaltipo  varchar(2),
    cidcodigo                  int,
    empcidade_ibge			   varchar(15),
    emplicencauso              varchar(255),
    emplogotipo                varchar(60),
    empcodigo_matriz           smallint,
    empim                      varchar(15),
    empendereco_numero         varchar(60),
    empendereco_complemento    varchar(60),
    empapelido                 varchar(120),
	empstatus			       char(1) default 'A',
    ponteiro 				   int,
    transa 					   int,
    usucodigo 				   smallint not null,
    ultatualizacao 			   timestamp default now(),
    constraint pk_emp primary key (empcnpj)
);
INSERT INTO empresa (empcodigo, empnome, empfantasia, empendereco, empbairro, empcidade, ufcodigo, empfone_ddd, empfone, empe_mail, empcep, empie, empcnpj, empcertificadodigital, empcertificadodigitaltipo, cidcodigo, empcidade_ibge, emplicencauso, emplogotipo, empcodigo_matriz, empim, empendereco_numero, empendereco_complemento, empapelido, empstatus, ponteiro, transa, usucodigo, ultatualizacao) 
  VALUES(1, 'LOGICOM TECNOLOGIA', 'LOGICOM', 'Trav. Antonio Barreto', 'Umarizal', 'Belém', 'PA', '91', '32229756', 'logicom@logicom.com.br', '66055050', '', '00696677000106', '', '', 0, '', '', 'logo', 1, '', '130', 'ED. VILLAGE OFFICE', 'LOGICOM TECNOLOGIA', 'A', 0, 0, 0, now());

create sequence if not exists vencodigo;
 create table vendedor (
    vencodigo                smallint not null,
    vennome                  varchar(60) not null,
    venapelido		     	 varchar(60) not null,
    venmeta                  numeric(15,2) default 0,
    vennivel                 smallint default 0 not null,
    venstatus                char(1) default 'A',
    ventipo                  char(1) default 'G', /* [G]arçom/Garçonete  */
    venemail                 varchar(120),
    ventelefone              varchar(60),
    vencpf 					 varchar(11),
    vencomissao              numeric(15,4) default 0.00,
    venlogin				 varchar(20) not null,
    vensenha                 varchar(255) not null,
    vennumero_origem         int,
    transa 					 int,
    usucodigo 				 smallint not null,
    ultatualizacao 			 timestamp default now(),
    constraint pk_ven primary key (vencodigo),
    constraint pk_venlogin unique(venlogin)
);
INSERT INTO vendedor (vencodigo, vennome, venapelido, venmeta, vennivel, venstatus, ventipo, venemail, ventelefone, vencpf, vencomissao, venlogin, vensenha, vennumero_origem, transa, usucodigo, ultatualizacao) values
                     (1, 'VENDEDOR', 'VENDEDOR', 0, 0, 'A', 'G', 'vendedor@logicom.com.br', '', '00000000000', 0.00, 'vendedor', '123', 0, 0, 0, now());

create sequence if not exists clmcodigo;
create table classmerc (
    clmcodigo              int not null,
    clmplano               varchar(20) not null,
    clmdescricao           varchar(30) not null,
    clmnivel               smallint not null,
    clmanalsint            char(1) not null,
    clmtipo				   char(1) not null default 'M' /* Menu */,
    clmpai                 int default 0 not null,
    clmstatus              char(1) DEFAULT 'A' not null,
    clmnumero_origem       int,
    transa 				   int,
    usucodigo 			   smallint not null,
    ultatualizacao 		   timestamp default now(),
    constraint pk_clm primary key (clmcodigo)
);
INSERT INTO classmerc (clmcodigo, clmplano, clmdescricao, clmnivel, clmanalsint, clmtipo, clmpai, clmstatus, clmnumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('clmcodigo'), '', '', 1, 'S', 'N', 0, 'A', null, 0, 0, now());
 
 
create sequence if not exists latcodigo;
create table local_atendimento (
  latcodigo smallint not null,
  lattipo char(1), /* M-Mesa   C-Comanda   S-Salão   I-Interno(para COPA/COZINHA) */
  latchave varchar(20) not null, /* A301  M001  B001  C123 */
  latnome varchar(30),
  latdescricao varchar(100),
  latnumero_origem int,
  latstatus char(1) not null, /* D-Disponivel  R-Reservada  O-Ocupado  I-Inativo  */
  usucodigo smallint not null,
  ultatualizacao timestamp default now() not null,
  constraint pk_lat primary key (latcodigo),
  constraint u_lat unique (latchave)  
);
INSERT INTO local_atendimento (latcodigo, lattipo, latchave, latnome, latdescricao, latstatus, usucodigo, ultatualizacao) VALUES(nextval('latcodigo'), 'M', 'M1', 'MESA 1', '', 'D', 1, now());
INSERT INTO local_atendimento (latcodigo, lattipo, latchave, latnome, latdescricao, latstatus, usucodigo, ultatualizacao) VALUES(nextval('latcodigo'), 'M', 'M2', 'MESA 2', '', 'D', 1, now());
INSERT INTO local_atendimento (latcodigo, lattipo, latchave, latnome, latdescricao, latstatus, usucodigo, ultatualizacao) VALUES(nextval('latcodigo'), 'M', 'M3', 'MESA 3', '', 'D', 1, now());
INSERT INTO local_atendimento (latcodigo, lattipo, latchave, latnome, latdescricao, latstatus, usucodigo, ultatualizacao) VALUES(nextval('latcodigo'), 'M', 'M4', 'MESA 4', '', 'D', 1, now());

INSERT INTO local_atendimento (latcodigo, lattipo, latchave, latnome, latdescricao, latstatus, usucodigo, ultatualizacao) VALUES(nextval('latcodigo'), 'C', 'C001', 'Comanda 001', '', 'D', 1, now());
INSERT INTO local_atendimento (latcodigo, lattipo, latchave, latnome, latdescricao, latstatus, usucodigo, ultatualizacao) VALUES(nextval('latcodigo'), 'C', 'C002', 'Comanda 002', '', 'D', 1, now());
INSERT INTO local_atendimento (latcodigo, lattipo, latchave, latnome, latdescricao, latstatus, usucodigo, ultatualizacao) VALUES(nextval('latcodigo'), 'C', 'C003', 'Comanda 003', '', 'D', 1, now());

INSERT INTO local_atendimento (latcodigo, lattipo, latchave, latnome, latdescricao, latstatus, usucodigo, ultatualizacao) VALUES(nextval('latcodigo'), 'I', 'COPA', 'Copa', '', 'O', 1, now());
INSERT INTO local_atendimento (latcodigo, lattipo, latchave, latnome, latdescricao, latstatus, usucodigo, ultatualizacao) VALUES(nextval('latcodigo'), 'I', 'COZINHA', 'Cozinha', '', 'O', 1, now());

 
create sequence if not exists cdpcodigo;
 create table cardapio (
    cdpcodigo              int not null,
    cdpplano               varchar(20) not null,
    cdptitulo			   varchar(100),
    cdpdescricao           varchar(250),
    cdpnivel               smallint not null,
    cdpsint            	   char(1) not null default 'S',
    cdptipo				   char(1) not null default 'M' /* Menu */,
    cdppai                 int default 0 not null,
    cdpstatus              char(1) DEFAULT 'A' not null,
    cdpnumero_origem       int,
    transa 				   int,
    usucodigo 			   smallint not null,
    ultatualizacao 		   timestamp default now(),
    constraint pk_cdp primary key (cdpcodigo)
);
INSERT INTO cardapio (cdpcodigo, cdpplano, cdptitulo, cdpdescricao, cdpnivel, cdpsint, cdptipo, cdppai, cdpstatus, cdpnumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('cdpcodigo'), '01', 'Café da Manhã', 'BREAKFAST SERVICE WITH COFFEE OR TEA', 0, 'A', 'M', 0, 'A', 0, 0, 0, now());
 
INSERT INTO cardapio (cdpcodigo, cdpplano, cdptitulo, cdpdescricao, cdpnivel, cdpsint, cdptipo, cdppai, cdpstatus, cdpnumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('cdpcodigo'), '02', 'Refeição Meal', '', 0, 'A', 'M', 0, 'A', 0, 0, 0, now());
  
 INSERT INTO cardapio (cdpcodigo, cdpplano, cdptitulo, cdpdescricao, cdpnivel, cdpsint, cdptipo, cdppai, cdpstatus, cdpnumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('cdpcodigo'), '03', 'Carnes', 'STEAKS', 0, 'A', 'M', 0, 'A', 0, 0, 0, now());

INSERT INTO cardapio (cdpcodigo, cdpplano, cdptitulo, cdpdescricao, cdpnivel, cdpsint, cdptipo, cdppai, cdpstatus, cdpnumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('cdpcodigo'), '10', 'Porções', '', 0, 'A', 'M', 0, 'A', 0, 0, 0, now());

INSERT INTO cardapio (cdpcodigo, cdpplano, cdptitulo, cdpdescricao, cdpnivel, cdpsint, cdptipo, cdppai, cdpstatus, cdpnumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('cdpcodigo'), '15', 'Bebidas', 'BEVERAGES', 0, 'A', 'M', 0, 'A', 0, 0, 0, now());


create sequence if not exists procodigo;
 create table produto (
  procodigo 				int not null,
  precodigo 				int,
  clmcodigo 				int,
  cdpcodigo 				int,
  embcodigo 				varchar(5),
  prequantidade 			numeric(15,6) NOT NULL,
  prodescricao 				varchar(60) not null,
  prodescricao_adicional 	varchar(250),
  propreco_venda 			numeric(15,4) DEFAULT 0.00,
  propreco_custo 			numeric(15,4) DEFAULT 0.00,
  procodbarra 				varchar (20),
  protipo 					char(1), /* V-Venda (venda direta), P-Porção/adicional (venda direta), C-Composição/Porção */
  proadicional 				char(1) default 'N',
  propreparo 				char(1) default 'N',
  latcodigo_preparo 		smallint, /* Local de preparo do produto - utilizado para definir local que deve ser exibido para preparo */
  prostatus 				char(1) default 'A',
  proimagem 				bytea,
  pronumero_origem 			int,
  transa 					int,
  usucodigo 				smallint not null,
  ultatualizacao 			timestamp default now(),
  constraint pk_pro primary key (procodigo)
);
INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, 1, 'UND', 1, 'Café/Chá Completo', '', 38.00, 38.00, '', 'V', 'N', 'S', (select latcodigo from local_atendimento where latchave = 'COZINHA'), 'A', null, 0, 0, 0, now());


INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, 2, 'UND', 1, 'Sopa de Legumes', '', 28.00, 28.00, '', 'V', 'N', 'S', (select latcodigo from local_atendimento where latchave = 'COZINHA'), 'A', null, 0, 0, 0, now());


INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, 3, 'UND', 1, 'Porção de Arroz', '', 13.00, 13.00, '', 'P', 'N', 'S', (select latcodigo from local_atendimento where latchave = 'COZINHA'), 'A', null, 0, 0, 0, now());


INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, 3, 'UND',  1, 'Porção de Arroz à Grega', '', 13.00, 13.00, '', 'P', 'S', 'N', (select latcodigo from local_atendimento where latchave = 'COZINHA'), 'A', null, 0, 0, 0, now());


INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo,latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, 3, 'UND',  1, 'Porção de Feijão', '', 13.00, 13.00, '', 'P', 'N', 'S', (select latcodigo from local_atendimento where latchave = 'COZINHA'), 'A', null, 0, 0, 0, now()); 


INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, 3, 'UND',  1, 'Porção de Purê', '', 13.00, 13.00, '', 'P', 'N', 'S',(select latcodigo from local_atendimento where latchave = 'COZINHA'), 'A', null, 0, 0, 0, now());


INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, 3, 'UND',  1, 'Porção de Farofa', '', 13.00, 13.00, '', 'P', 'N', 'S', (select latcodigo from local_atendimento where latchave = 'COZINHA'), 'A', null, 0, 0, 0, now());


INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, 3, 'UND',  1, 'Porção de Farofa c/ Ovos', '', 13.00, 13.00, '', 'P', 'N', 'S', (select latcodigo from local_atendimento where latchave = 'COZINHA'), 'A', null, 0, 0, 0, now());


INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, 3, 'UND', 1, 'Porção de Ovos c/ Bacon', '', 13.00, 13.00, '', 'P', 'N', 'S', (select latcodigo from local_atendimento where latchave = 'COZINHA'), 'A', null, 0, 0, 0, now());


INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, 3, 'UND', 1, 'Porção de Batata Frita', '', 13.00, 13.00, '', 'P', 'N', 'S', (select latcodigo from local_atendimento where latchave = 'COZINHA'), 'A', null, 0, 0, 0, now());
 
INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, 3, 'UND', 1, 'Porção de Mingau de Tapioca', '', 13.00, 13.00, '', 'P', 'N', 'S', (select latcodigo from local_atendimento where latchave = 'COZINHA'), 'A', null, 0, 0, 0, now());
 
INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, 3, 'UND', 1, 'Porção de Mingau de Aveia', '', 13.00, 13.00, '', 'P', 'N', 'S', (select latcodigo from local_atendimento where latchave = 'COZINHA'), 'A', null, 0, 0, 0, now()); 
 
INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, 3, 'UND', 1, 'Porção de Frutas', '3 unidades ou fatias', 13.00, 13.00, '', 'P', 'N', 'S', (select latcodigo from local_atendimento where latchave = 'COZINHA'), 'A', null, 0, 0, 0, now());


INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, 3, 'UND', 1, 'Porção de Banana Empanada', '', 13.00, 13.00, '', 'P', 'N', 'S', (select latcodigo from local_atendimento where latchave = 'COZINHA'), 'A', null, 0, 0, 0, now());


 INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, 4, 'UND', 1, 'Filé à Presidente', 'à milanesa recheado com queijo e presunto, arroz branco e banana empanada', 68.00, 68.00, '', 'P', 'N', 'S', (select latcodigo from local_atendimento where latchave = 'COZINHA'), 'A', null, 0, 0, 0, now());

INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo,  prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, (select cdpcodigo from cardapio where cdptitulo = 'Bebidas'), 'UND', 1, 'Água Mineral 300ml', '', 6.00, 2.00, '', 'V', 'N', 'N', (select latcodigo from local_atendimento where latchave = 'COPA'), 'A', null, 0, 0, 0, now());


INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, (select cdpcodigo from cardapio where cdptitulo = 'Bebidas'), 'UND', 1, 'Água Mineral com Gás 300ml', '', 6.00, 2.00, '', 'V', 'N', 'N', (select latcodigo from local_atendimento where latchave = 'COPA'), 'A', null, 0, 0, 0, now());

INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, (select cdpcodigo from cardapio where cdptitulo = 'Bebidas'), 'UND', 1, 'Regrigerante Coca-Cola Lata 250ml', '', 7.00, 2.50, '', 'V', 'N', 'N', (select latcodigo from local_atendimento where latchave = 'COPA'), 'A', null, 0, 0, 0, now());


INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, (select cdpcodigo from cardapio where cdptitulo = 'Bebidas'), 'UND', 1, 'Regrigerante Guaraná Antarctica Lata 250ml', '', 7.00, 2.50, '', 'V', 'N', 'N', (select latcodigo from local_atendimento where latchave = 'COPA'), 'A', null, 0, 0, 0, now());


INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, (select cdpcodigo from cardapio where cdptitulo = 'Bebidas'), 'UND', 1, 'Suco de Goiaba', '', 11.00, 2.50, '', 'V', 'N', 'N', (select latcodigo from local_atendimento where latchave = 'COPA'), 'A', null, 0, 0, 0, now());


INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, (select cdpcodigo from cardapio where cdptitulo = 'Bebidas'), 'UND', 1, 'Suco de Acerola', '', 11.00, 2.50, '', 'V', 'N', 'N', (select latcodigo from local_atendimento where latchave = 'COPA'), 'A', null, 0, 0, 0, now());


INSERT INTO produto (procodigo, precodigo, clmcodigo, cdpcodigo, embcodigo, prequantidade, prodescricao, prodescricao_adicional, propreco_venda, propreco_custo, procodbarra, protipo, proadicional, propreparo, latcodigo_preparo, prostatus, proimagem, pronumero_origem, transa, usucodigo, ultatualizacao) 
  VALUES(nextval('procodigo'), null, null, (select cdpcodigo from cardapio where cdptitulo = 'Bebidas'), 'UND', 1, 'Vitamina de Frutas', '', 13.00, 4.50, '', 'V', 'N', 'N', (select latcodigo from local_atendimento where latchave = 'COPA'), 'A', null, 0, 0, 0, now());
  
 /*select
   p.procodigo 
  ,p.prodescricao
  ,p.prodescricao_adicional
  ,p.propreco_venda 
  ,c.cdpcodigo 
  ,c.cdptitulo 
  ,c.cdpdescricao   
   from produto p
   inner join cardapio c
   on (p.cdpcodigo = c.cdpcodigo)
   where p.prostatus = 'A'
   order by c.cdpplano,p.prodescricao;*/


create sequence if not exists pcpcodigo;
create table produto_composicao (
  pcpcodigo int not null,
  procodigo_pai int not null,  
  procodigo int not null,
  pcpadicional char(1) default 'N' not null,
  pcpisento_cobranca char(1) default 'N' not null,
  pcpstatus char(1) default 'A' not null,
  pcpinsercao timestamp default now(),
  usucodigo smallint not null,
  ultatualizacao timestamp default now(),
  constraint pk_pcp primary key (pcpcodigo),
  constraint fk_pro_pcp foreign key (procodigo_pai) references produto (procodigo)
);
INSERT INTO produto_composicao (pcpcodigo, procodigo_pai, procodigo, pcpadicional, pcpisento_cobranca, pcpstatus, pcpinsercao, usucodigo, ultatualizacao) 
  VALUES(nextval('pcpcodigo'), (select procodigo from produto where prodescricao = 'Filé à Presidente'), (select procodigo from produto where prodescricao = 'Porção de Arroz'), 'N', 'N', 'A', now(), 0, now());
 
INSERT INTO produto_composicao (pcpcodigo, procodigo_pai, procodigo, pcpadicional, pcpisento_cobranca, pcpstatus, pcpinsercao, usucodigo, ultatualizacao) 
  VALUES(nextval('pcpcodigo'), (select procodigo from produto where prodescricao = 'Filé à Presidente'), (select procodigo from produto where prodescricao = 'Porção de Banana Empanada'), 'N', 'N', 'A', now(), 0, now());
 
/*select * from produto where procodigo = 9

select procodigo from produto where prodescricao = 'Porção de Purê' */
 
create sequence if not exists pcsbcodigo;
create table produto_composicao_substituto (
  pcsbcodigo int not null,
  pcpcodigo int not null,
  procodigo_pai int not null,
  procodigo int not null,
  pcsbvalor_adicional numeric(15,2) default 0.00,
  pcsbstatus char(1) default 'A' not null,
  pcsbinsercao timestamp default now(),
  usucodigo smallint not null,
  ultatualizacao timestamp default now(),
  constraint pk_pcsb primary key (pcsbcodigo)
  /*constraint fk_pcsb_pcp foreign key (procodigo_pai) references produto_composicao (procodigo)*/  
);
/* troca de arroz por arroz à grega*/
INSERT INTO produto_composicao_substituto (pcsbcodigo, pcpcodigo, procodigo_pai, procodigo, pcsbvalor_adicional, pcsbstatus, pcsbinsercao, usucodigo, ultatualizacao) 
  VALUES(nextval('pcsbcodigo'), 
  (select pcpcodigo from produto_composicao pc where procodigo_pai = (select procodigo from produto where prodescricao = 'Filé à Presidente') and procodigo = (select procodigo from produto where prodescricao = 'Porção de Arroz'))
  ,(select procodigo from produto_composicao pc where procodigo_pai = (select procodigo from produto where prodescricao = 'Filé à Presidente') and procodigo = (select procodigo from produto where prodescricao = 'Porção de Arroz'))
  ,(select procodigo from produto where prodescricao = 'Porção de Arroz à Grega') 
  , 0.00, 'A', now(), 0, now());
 
/*(select pcpcodigo from produto_composicao pc where procodigo_pai = (select procodigo from produto where prodescricao = 'Filé à Presidente') and procodigo = (select procodigo from produto where prodescricao = 'Porção de Arroz'))*/ 

 /* troca de banana empanda por purê*/
 /*INSERT INTO produto_composicao_substituto (pcsbcodigo, pcpcodigo, procodigo_pai, procodigo, pcsbvalor_adicional, pcsbstatus, pcsbinsercao, usucodigo, ultatualizacao) 
  VALUES(nextval('pcsbcodigo'), 2, 21, 12, 0.00, 'A', now(), 0, now());*/
 
/* troca de banana empanda por purê*/
 INSERT INTO produto_composicao_substituto (pcsbcodigo, pcpcodigo, procodigo_pai, procodigo, pcsbvalor_adicional, pcsbstatus, pcsbinsercao, usucodigo, ultatualizacao) 
  VALUES(nextval('pcsbcodigo'), 
  (select pcpcodigo from produto_composicao pc where procodigo_pai = (select procodigo from produto where prodescricao = 'Filé à Presidente') and procodigo = (select procodigo from produto where prodescricao = 'Porção de Banana Empanada'))
  ,(select procodigo from produto_composicao pc where procodigo_pai = (select procodigo from produto where prodescricao = 'Filé à Presidente') and procodigo = (select procodigo from produto where prodescricao = 'Porção de Banana Empanada'))
  ,(select procodigo from produto where prodescricao = 'Porção de Purê')
  , 0.00, 'A', now(), 0, now());
 
  /* troca de banana empanda por batata frita */
/* INSERT INTO produto_composicao_substituto (pcsbcodigo, pcpcodigo, procodigo_pai, procodigo, pcsbvalor_adicional, pcsbstatus, pcsbinsercao, usucodigo, ultatualizacao) 
  VALUES(nextval('pcsbcodigo'), 2, 21, 16, 0.00, 'A', now(), 0, now()); */
 
 INSERT INTO produto_composicao_substituto (pcsbcodigo, pcpcodigo, procodigo_pai, procodigo, pcsbvalor_adicional, pcsbstatus, pcsbinsercao, usucodigo, ultatualizacao) 
  VALUES(nextval('pcsbcodigo'), 
  (select pcpcodigo from produto_composicao pc where procodigo_pai = (select procodigo from produto where prodescricao = 'Filé à Presidente') and procodigo = (select procodigo from produto where prodescricao = 'Porção de Banana Empanada'))
  ,(select procodigo from produto_composicao pc where procodigo_pai = (select procodigo from produto where prodescricao = 'Filé à Presidente') and procodigo = (select procodigo from produto where prodescricao = 'Porção de Banana Empanada'))
  ,(select procodigo from produto where prodescricao = 'Porção de Batata Frita') 
  , 0.00, 'A', now(), 0, now());


/*
 * comanda não exibe opcao de mesa
 * mesa exibe opção de mesa e apartamento
 * 
 */

create sequence if not exists cmdcodigo;
create table comanda (
  cmdcodigo int not null,
  latcodigo smallint not null,
  cmdapartamento int,
  usucodigo_abertura smallint not null,
  cmdobservacao varchar(250),
  cmdvalor_total numeric(15,2) default 0.00,
  cmdvalor_taxa_servico numeric(15,2) default 0.00,
  cmdvalor_pago numeric(15,2) default 0.00,
  cmdstatus char(1) not null, /* A-Aberta  F-Fechada/Paga/Faturada  C-Cancelada  U-Unificada  X-Excluída */
  cmdabertura timestamp default now(),
  cmdfechamento timestamp default now(),
  cmdnumero_origem int, /* Se Hotel 'regficha' / Se SCEF 'pescodigo' */
  transa int,
  usucodigo smallint not null,
  ultatualizacao timestamp default now(),
  constraint pk_cmd primary key (cmdcodigo),
  constraint fk_usu_cmd foreign key (usucodigo_abertura) references usuario (usucodigo)
);

/* exemplo de insert para apto 301 */
/*INSERT INTO comanda (cmdcodigo, latcodigo, cmdapartamento, usucodigo_abertura, cmdobservacao, cmdvalor_total, cmdvalor_taxa_servico, cmdvalor_pago, cmdstatus, cmdabertura, cmdfechamento, cmdnumero_origem, transa, usucodigo, ultatualizacao) values
  (nextval('cmdcodigo'), 1, 301, 1, 'Observação de teste', 0.00, 0.00, 0.00, 'A', now(), now(), 0, 0, 1, now());
 
 INSERT INTO comanda (cmdcodigo, latcodigo, cmdapartamento, usucodigo_abertura, cmdobservacao, cmdvalor_total, cmdvalor_taxa_servico, cmdvalor_pago, cmdstatus, cmdabertura, cmdfechamento, cmdnumero_origem, transa, usucodigo, ultatualizacao) values
  (nextval('cmdcodigo'), 5, null, 1, 'Exemplo de comanda', 0.00, 0.00, 0.00, 'A', now(), now(), 0, 0, 1, now()); */
 
/*create or replace function func_comanda_upsert (_cmdcodigo int, _latcodigo smallint, _cmdapartamento int, _vencodigo_abertura smallint, _cmdobservacao varchar(250), _cmdstatus char(1), _cmdnumero_origem int, _transa int, _usucodigo smallint)
  returns int
  language plpgsql
  as
  $$
  begin
	  if (_cmdcodigo > 0) then
	     UPDATE comanda SET latcodigo=_latcodigo, cmdapartamento=_cmdapartamento, vencodigo_abertura=_vencodigo_abertura, cmdobservacao=_cmdobservacao, cmdvalor_taxa_servico=_cmdvalor_taxa_servico, cmdstatus=_cmdstatus, cmdnumero_origem=_cmdnumero_origem, usucodigo=_usucodigo, ultatualizacao=now() WHERE cmdcodigo=_cmdcodigo and cmdstatus <> 'A';
	  else
	     _cmdcodigo = nextval('cmdcodigo');
	     INSERT INTO comanda (cmdcodigo, latcodigo, cmdapartamento, vencodigo_abertura, cmdobservacao, cmdvalor_total, cmdvalor_taxa_servico, cmdvalor_pago, cmdstatus, cmdabertura, cmdfechamento, cmdnumero_origem, transa, usucodigo, ultatualizacao) values 
	                         (cast(_cmdcodigo as int), cast(_latcodigo as smallint), cast(_cmdapartamento as int), cast(_vencodigo_abertura as smallint), cast(_cmdobservacao as varchar(250)), cast(0.00 as numeric(15,2)), cast(0.00 as numeric(15,2)), cast(0.00 as numeric(15,2)), cast(_cmdstatus as char(1)), now(), null, cast(_cmdnumero_origem as int), cast(_transa as int), cast(_usucodigo as smallint), now());
	  end if;
	 return _cmdcodigo;
  end;
 $$;*/


/*select
  la.latcodigo
 ,la.lattipo
 ,la.latchave
 ,la.latnome
 ,la.latstatus
 ,c.cmdcodigo
 ,c.cmdapartamento
 ,c.cmdabertura 
from comanda c
  left join local_atendimento la
  on (c.latcodigo = la.latcodigo)
where c.cmdabertura between :dini and :dfim
  and c.cmdstatus = 'A';*/


create sequence if not exists cmdicodigo; 
create table comanda_item (
  cmdicodigo int not null,
  cmdcodigo int not null,
  usucodigo_atendimento smallint not null,	
  procodigo int not null,
  precodigo int,
  cmdivalor numeric(15,2) not null,
  cmdivalor_pago numeric(15,2) default 0.00,
  cmdiobservacao varchar(255),
  cmdistatus char(1) default 'B' not null, /* B-Bloqueado  A-Aberto  W-EmPreparoCozinha  Z-FimPreparoCozinha  E-Entregue  X-Excluído  P-Pago/F-Faturado */
  cmdinumero_origem int, /* Quando for Hotelaria regficha do hóspede ou quando SCEF pescodigo */
  cmdipreparo char(1), /* S-Sim  N-Não */
  usucodigo smallint not null,
  ultatualizacao timestamp default now(),
  constraint pk_cmdi primary key (cmdicodigo),
  constraint fk_cmd_cmdi foreign key (cmdcodigo) references comanda (cmdcodigo),
  constraint fk_usu_cmdi foreign key (usucodigo) references usuario (usucodigo)
);


create sequence if not exists ciccodigo;
create table comanda_item_composicao (
  ciccodigo int not null,
  cmdicodigo int not null,
  procodigo int not null,
  procodigo_composicao int not null,
  usucodigo smallint not null,
  ultatualizacao timestamp default now(),
  constraint pk_cic primary key (ciccodigo),
  constraint pk_cmdi_cic foreign key (cmdicodigo) references comanda_item (cmdicodigo)
);

/*
select
   la.latnome
  ,min(cmil.cmilinsercao) as cmilinsercao
  ,count(ci.cmdicodigo) as qtd_itens
  from comanda c
  inner join comanda_item ci
  on (c.cmdcodigo = ci.cmdcodigo)
  inner join local_atendimento la
  on (c.latcodigo = la.latcodigo)
  inner join comanda_item_log cmil
  on (ci.cmdicodigo = cmil.cmdicodigo)
  inner join tipo_log_item tli 
  on (tli.tlicodigo = cmil.tlicodigo)
  where ci.cmdipreparo = 'S'
  and tli.tlichave = 'PREPLIB'
  group by la.latnome; */
  
  
/*
select
    p.procodigo
   ,p.prodescricao
   ,ci.cmdiobservacao
   ,ci.cmdistatus
   ,cmil.cmilinsercao
  from comanda c
  inner join comanda_item ci
  on (c.cmdcodigo = ci.cmdcodigo)
  inner join produto p
  on (ci.procodigo = p.procodigo)
  inner join comanda_item_log cmil
  on (ci.cmdicodigo = cmil.cmdicodigo)
  left join tipo_log_item tli 
  on (tli.tlicodigo = cmil.tlicodigo)
  where ci.cmdipreparo = 'S'
  and tli.tlichave = 'PREPLIB'
  order by cmil.cmilinsercao;
  */

/* select * from comanda_item; */


create sequence if not exists tlicodigo;
create table tipo_log_item (
  tlicodigo smallint not null,
  tlinome varchar(50) not null,
  tligrupo varchar(20) not null,
  tlichave varchar(10) not null,
  tliseq smallint,
  tliobrigatorio char(1) default 'N' not null,
  tlistatus char(1) default 'A' not null,
  usucodigo smallint not null,
  ultatualizacao timestamp default now() not null,
  constraint pk_tli primary key (tlicodigo),
  constraint u_tli unique (tlichave)
);
INSERT INTO tipo_log_item (tlicodigo, tlinome, tligrupo, tlichave, tliseq, tliobrigatorio, tlistatus, usucodigo, ultatualizacao) values
  (nextval('tlicodigo'), 'Pré-lançamento', 'PP', 'PRELANC', 1, 'N', 'A', 1, now());
INSERT INTO tipo_log_item (tlicodigo, tlinome, tligrupo, tlichave, tliseq, tliobrigatorio, tlistatus, usucodigo, ultatualizacao) values
  (nextval('tlicodigo'), 'Liberado para preparo', 'PP', 'PREPLIB', 1, 'N', 'A', 1, now());
INSERT INTO tipo_log_item (tlicodigo, tlinome, tligrupo, tlichave, tliseq, tliobrigatorio, tlistatus, usucodigo, ultatualizacao) values
  (nextval('tlicodigo'), 'Preparo iniciado', 'PP', 'PREPINI', 2, 'N', 'A', 1, now()); 
INSERT INTO tipo_log_item (tlicodigo, tlinome, tligrupo, tlichave, tliseq, tliobrigatorio, tlistatus, usucodigo, ultatualizacao) values
  (nextval('tlicodigo'), 'Preparo finalizado', 'PP', 'PREPFIN', 3, 'N', 'A', 1, now());
INSERT INTO tipo_log_item (tlicodigo, tlinome, tligrupo, tlichave, tliseq, tliobrigatorio, tlistatus, usucodigo, ultatualizacao) values
  (nextval('tlicodigo'), 'Disponível para retirada/servir', 'PP', 'DISPRET', 4, 'N', 'A', 1, now());
INSERT INTO tipo_log_item (tlicodigo, tlinome, tligrupo, tlichave, tliseq, tliobrigatorio, tlistatus, usucodigo, ultatualizacao) values
  (nextval('tlicodigo'), 'Retirada realizada', 'PP', 'RETREAL', 5, 'N', 'A', 1, now());
INSERT INTO tipo_log_item (tlicodigo, tlinome, tligrupo, tlichave, tliseq, tliobrigatorio, tlistatus, usucodigo, ultatualizacao) values
  (nextval('tlicodigo'), 'Entrega finalizada', 'PP', 'ENTFIM', 6, 'N', 'A', 1, now());
INSERT INTO tipo_log_item (tlicodigo, tlinome, tligrupo, tlichave, tliseq, tliobrigatorio, tlistatus, usucodigo, ultatualizacao) values
  (nextval('tlicodigo'), 'Cancelamento Loja', 'CC', 'CANCLOJA', 0, 'N', 'A', 1, now());
INSERT INTO tipo_log_item (tlicodigo, tlinome, tligrupo, tlichave, tliseq, tliobrigatorio, tlistatus, usucodigo, ultatualizacao) values
  (nextval('tlicodigo'), 'Cancelamento Cliente', 'CC', 'CANCCLI', 0, 'N', 'A', 1, now());
 INSERT INTO tipo_log_item (tlicodigo, tlinome, tligrupo, tlichave, tliseq, tliobrigatorio, tlistatus, usucodigo, ultatualizacao) values
  (nextval('tlicodigo'), 'Bloqueio Loja', 'CC', 'BLOQLOJA', 0, 'N', 'A', 1, now());
INSERT INTO tipo_log_item (tlicodigo, tlinome, tligrupo, tlichave, tliseq, tliobrigatorio, tlistatus, usucodigo, ultatualizacao) values
  (nextval('tlicodigo'), 'Pagamento/Faturamento', 'PG', 'PAGITEM', 1, 'N', 'A', 1, now());
INSERT INTO tipo_log_item (tlicodigo, tlinome, tligrupo, tlichave, tliseq, tliobrigatorio, tlistatus, usucodigo, ultatualizacao) values
  (nextval('tlicodigo'), 'Cancelamento Loja Negado', 'CC', 'CANCLJNEG', 0, 'N', 'A', 1, now());

 
 
/* INSERT INTO comanda_item_log (cmilcodigo, cmdicodigo, tlicodigo, usucodigo, cmilinsercao) VALUES(nextval('cmilcodigo'), @cmdicodigo, @tlicodigo, 0, now()); 


INSERT INTO comanda_item_log (cmilcodigo, cmdicodigo, tlicodigo, usucodigo, cmilinsercao) VALUES(nextval('cmilcodigo'), @cmdicodigo, (select tlicodigo from tipo_log_item where tlichave = @tlichave), 0, now());
*/

create sequence if not exists cmipcodigo;
create table comanda_item_preparo (
  cmipcodigo int not null,
  cmdicodigo int not null,
  tlicodigo smallint,
  latcodigo smallint,
  usucodigo smallint not null,
  ultatualizacao timestamp default now(),
  constraint pk_cmip primary key (cmipcodigo),
  constraint pk_cmdi_cmip foreign key (cmdicodigo) references comanda_item (cmdicodigo)
);

create sequence if not exists cmilcodigo;
create table comanda_item_log (
  cmilcodigo int not null,
  cmdicodigo int not null,
  tlicodigo int not null,
  usucodigo smallint not null,
  usucodigo smallint not null,
  cmilinsercao timestamp default now() not null,
  constraint pk_cmil primary key (cmilcodigo),
  constraint fk_cmdi_cmil foreign key (cmdicodigo) references comanda_item (cmdicodigo)
);

/*LOGICOM_STATUS_FIELD E STATUS FIELD ITENS*/


INSERT INTO LOGICOM_STATUS_FIELDS (LSFCODIGO, LSFTABELA, LSFFIELD, LSFTAMANHO, LSFDESCRICAO, LTBCODIGO) VALUES (1, 'COMANDA', 'CMDSTATUS', 1, 'Status da Comanda', NULL);



INSERT INTO LOGICOM_STATUS_FIELDS_ITEM (LSFCODIGO, LSIVALOR, LSISEQUENCIA, LSIDESCRICAO, LSIOBSERVACAO) VALUES (1, 'A', 1, 'Aberto', 'Cabecalho de Comanda Aberto');
INSERT INTO LOGICOM_STATUS_FIELDS_ITEM (LSFCODIGO, LSIVALOR, LSISEQUENCIA, LSIDESCRICAO, LSIOBSERVACAO) VALUES (1, 'F', 2, 'Fechado', 'Cabecalho de Comanda Fechado');
INSERT INTO LOGICOM_STATUS_FIELDS_ITEM (LSFCODIGO, LSIVALOR, LSISEQUENCIA, LSIDESCRICAO, LSIOBSERVACAO) VALUES (1, 'C', 3, 'Cancelado', 'Cabecalho de Comanda Cancelado');
INSERT INTO LOGICOM_STATUS_FIELDS_ITEM (LSFCODIGO, LSIVALOR, LSISEQUENCIA, LSIDESCRICAO, LSIOBSERVACAO) VALUES (1, 'X', 4, 'Excluido', 'Excluido');


INSERT INTO LOGICOM_STATUS_FIELDS (LSFCODIGO, LSFTABELA, LSFFIELD, LSFTAMANHO, LSFDESCRICAO, LTBCODIGO) VALUES (2, 'COMANDA_ITEM', 'CMDISTATUS', 1, 'Status do Item do Cabecalho de Comanda', NULL);



INSERT INTO LOGICOM_STATUS_FIELDS_ITEM (LSFCODIGO, LSIVALOR, LSISEQUENCIA, LSIDESCRICAO, LSIOBSERVACAO) VALUES (2, 'A', 1, 'Ativo', 'Item Ativo');

INSERT INTO LOGICOM_STATUS_FIELDS_ITEM (LSFCODIGO, LSIVALOR, LSISEQUENCIA, LSIDESCRICAO, LSIOBSERVACAO) VALUES (2, 'X', 2, 'Excluido', 'Item Cancelado');
