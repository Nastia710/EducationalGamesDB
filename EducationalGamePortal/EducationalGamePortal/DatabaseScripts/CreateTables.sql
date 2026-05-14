DROP TABLE IF EXISTS user_achievements CASCADE; 
DROP TABLE IF EXISTS game_sessions CASCADE;    
DROP TABLE IF EXISTS achievements CASCADE;     
DROP TABLE IF EXISTS levels CASCADE; 
DROP TABLE IF EXISTS games CASCADE;
DROP TABLE IF EXISTS users CASCADE;   

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    nickname VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    registration_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE games (
    id SERIAL PRIMARY KEY,
    title VARCHAR(100) NOT NULL,
    subject VARCHAR(100) NOT NULL,
    description TEXT
);

CREATE TABLE levels (
    id SERIAL PRIMARY KEY,
    game_id INT NOT NULL REFERENCES games(id) ON DELETE CASCADE,
    level_number INT NOT NULL,
    title VARCHAR(100) NOT NULL,
    difficulty VARCHAR(20) CHECK (difficulty IN ('Easy', 'Medium', 'Hard')),
    max_score INT NOT NULL CHECK (max_score > 0),
    UNIQUE (game_id, level_number)
);

CREATE TABLE game_sessions (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES users(id) ON DELETE CASCADE,
    level_id INT REFERENCES levels(id) ON DELETE CASCADE,
    score INT NOT NULL DEFAULT 0 CHECK (score >= 0),
    time_spent INT NOT NULL DEFAULT 0 CHECK (time_spent >= 0),
    is_completed BOOLEAN NOT NULL DEFAULT FALSE,
    played_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE achievements (
    id SERIAL PRIMARY KEY,
    title VARCHAR(100) NOT NULL,
    description TEXT,
    criteria_type VARCHAR(50) NOT NULL,
    criteria_value INT NOT NULL
);

CREATE TABLE user_achievements (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES users(id) ON DELETE CASCADE,
    achievement_id INT REFERENCES achievements(id) ON DELETE CASCADE,
    unlocked_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UNIQUE (user_id, achievement_id)
);
